SocketProxy = class("SocketProxy",nil,true)

local IgnoreWaitingViewOpcode = {
	Opcode.C2GS_Heartbeat,
	Opcode.GS2C_Heartbeat,
	Opcode.GS2C_RedPoint,
	Opcode.C2GS_RedPoint,
	Opcode.GS2C_RedPointRemind,
	Opcode.C2GS_RedPointRemind,
	Opcode.GS2C_UpdateTineng,
	Opcode.C2GS_UpdateTineng,
	Opcode.GS2C_BuildMatchReady,
	Opcode.C2GS_BuildMatchReady,
	Opcode.GS2C_RoundResult,
	Opcode.GS2C_ChatFriendsRemind,
	Opcode.GS2C_Pmd,
	Opcode.C2GS_Pmd,
	Opcode.GS2C_PushChat,
	Opcode.C2GS_PushChat,
	Opcode.GS2C_RankedRaceRecord,
	Opcode.C2GS_RankedRaceRecord,
	Opcode.GS2C_UpdateStep,
	Opcode.C2GS_UpdateStep,
	Opcode.C2GS_GetIndexMessage,
	Opcode.GS2C_GetIndexMessage,
	Opcode.GS2C_PmdControl,
	Opcode.C2GS_PmdControl,
	Opcode.C2GS_RemainRounds,
	Opcode.GS2C_RemainRounds,
	Opcode.C2GS_PayloadWrite,
	Opcode.C2GS_LeaveMatch,
	Opcode.C2GS_ReportAction,
	Opcode.C2GS_TiedTel,
	Opcode.C2GS_HelpStrongerData,
	Opcode.C2GS_HelpJumpData,
	Opcode.C2GS_EnterTelPage,
	Opcode.GS2C_FormationZhanliChanged,
	Opcode.C2GS_NewPlayerCareerChapterRedpointInfo,
	Opcode.GS2C_NewPlayerCareerChapterRedpointInfo,
	Opcode.C2GS_EnterRealNameRegisation,
	Opcode.C2GS_ReportMsg,
	Opcode.GS2C_ReportMsg,
	Opcode.C2GS_ChangeBuff,
	Opcode.GS2C_ChangeBuff,
	Opcode.C2GS_Share,
	Opcode.C2GS_EasterEggControl
}

local function IsIgnoreWaitingViewOpcode(opcode)
	for _,v in pairs(IgnoreWaitingViewOpcode) do
		if v == opcode then
			return true
		end
	end

	return false
end

function SocketProxy:ctor()
	self.reconnectFailedTime = 0
	self.mMsgList = {};
	self.errorMsgList = {}; -- 请求失败是执行的列表
end

function SocketProxy:BeginConnect(host,port,cb)
	self.activeDisconnect = false
	WindowMgr:ShowWaiting(nil,true)
	LuaHelper.GetGSNetManager():BeginConnect(host,port,NetManager.NetDelegate(cb))
end

function SocketProxy:SendPacket(opcode,proto)
	if self.activeDisconnect then
		return
	end
	if type(proto) ~= "table" or not proto.SerializeToString then
		logError("proto is not table or haven't SerializeToString function")
	end
	LuaHelper.GetGSNetManager():SendPacket(Packet(opcode,proto:SerializeToString()))

	if not IsIgnoreWaitingViewOpcode(opcode) then
		WindowMgr:ShowWaiting()
	end
	
	if opcode ~= Opcode.C2GS_Heartbeat then
		-- self.lastOpCode = opcode
	end
end

local function SetProtoHandle(self,opcode,cb,viewOrModel,registered)
	if type(cb) ~= "function" then
		logError("SocketProxy: need a function for callback")
		return
	end

	for _,v in pairs(self.mMsgList) do
		if v.cb == cb and v.opcode == opcode then
			logError("SocketProxy: xxxx")
			return
		end
	end
	table.insert(self.mMsgList,{opcode = opcode,cb = cb,registered = registered,viewOrModel = viewOrModel})
end

local function SetErrorHandle(self,opcode,cb,viewOrModel,registered)
	if type(cb) ~= "function" then
		logError("SocketProxy: need a function for callback")
		return
	end

	for _,v in pairs(self.errorMsgList) do
		if v.cb == cb and v.opcode == opcode then
			return
		end
	end
	table.insert(self.errorMsgList,{opcode = opcode,cb = cb,registered = registered,viewOrModel = viewOrModel})
	self.lastOpCode = opcode
end

-- SendPacket 之后只错误code回调是处理一次cb
function SocketProxy:HandleError(opcode,cb)
	SetErrorHandle(self,opcode,cb,nil,false)
end

-- SendPacket 之后只处理一次cb
function SocketProxy:HandleProto(opcode,cb)
	SetProtoHandle(self,opcode,cb,nil,false)
end

-- SendPacket 之后，每次收到一样的消息都会cb
function SocketProxy:RegisterProto(opcode,viewOrModel,cb)
	if type(viewOrModel) ~= "table" then
		logError("SocketProxy: need a table(viewOrModel) for register")
		return
	end

	for _,v in pairs(self.mMsgList) do
		if v.opcode == opcode and v.viewOrModel == viewOrModel and v.registered then
			logError("SocketProxy: register the same viewOrModel")
			return
		end
	end

	if not table.contains(IgnoreWaitingViewOpcode,opcode) then
		table.insert(IgnoreWaitingViewOpcode,opcode)
	end

	SetProtoHandle(self,opcode,cb,viewOrModel,true)
end

function SocketProxy:UnregisterProtoByViewOrModel(viewOrModel)
	if type(viewOrModel) ~= "table" then
		logError("SocketProxy: need a table(viewOrModel) for unregister")
		return
	end
	table.eraseBy(self.mMsgList,function (v)
		return viewOrModel == v.viewOrModel and v.registered
	end)
end

function SocketProxy:UnregisterProtoByOpcode(opcode)
	table.eraseBy(self.mMsgList,function (v)
		return opcode == v.opcode and v.registered
	end)
end

function SocketProxy:DistributeError(msg)
	if not msg then
		msg = {opCode = self.lastOpCode}
	end
	table.foreach(self.lastErrorMsgList,function(_,v)
		if msg.opCode == v.opcode then
			--log(string.format("SocketProxy:Distribute [0x%04X]",opcode))
			v.cb(msg)
		end
	end)
	-- table.eraseBy(self.lastErrorMsgList,function (v)
	-- 	return msg.opCode == v.opcode and not v.registered
	-- end)

	table.eraseBy(self.mLastMsgList,function (v)
		return not v.registered
	end)
	table.eraseBy(self.lastErrorMsgList,function (v)
		return not v.registered
	end)
end

function SocketProxy:Distribute(opcode,bytes)
	if not IsIgnoreWaitingViewOpcode(opcode) then
		local ignore = nil
		if opcode ~= Opcode.GS2C_ERRORCODE then
			table.foreach(self.mMsgList,function(_,v)
				if opcode == v.opcode then
					if v.registered then
						ignore = true
					end
				end
			end)
		end
		WindowMgr:HideWaiting(ignore)
	end

	local msg = Opcode:ParseGS2C(opcode,bytes):ConvertToNormalTable()
	if msg then
		if opcode == Opcode.GS2C_ERRORCODE then
			logError(string.format("server GS2C_ERRORCODE error code: [%d] in Opcode.%s  0x[%04X]",msg.code,GetOpcodeName(msg.opCode),msg.opCode))
			SocketProxy:ClearAllHandleProto()
			self:DistributeError(msg)
			self:errorCommHandle(msg)
			-- clear all handle proto, server cannot get the right GS2C_xxxx opcode
		else
			local cbs = {}
			table.foreach(self.mMsgList,function(_, v)
				if opcode == v.opcode then
					--log(string.format("SocketProxy:Distribute [0x%04X]",opcode))
					table.insert(cbs, v.cb)
					-- v.cb(msg)
				end
			end)
			table.foreach(cbs, function(_, v)
				v(msg)
			end)

			table.eraseBy(self.mMsgList,function (v)
				return opcode == v.opcode and not v.registered
			end)

			table.eraseBy(self.errorMsgList,function (v)
				return opcode == v.opcode and not v.registered
			end)
		end
	else
		logError(string.format("received opcode 0x[%04X] without proto lua file! please make new proto!",opcode))
	end
end

function SocketProxy:ShowLostConnection()
	WindowMgr:HideWaiting()
	self.reconnectFailedTime = 0
	self:CloseSocket()
	local param = {
		text = "您已与服务器断开连接，请返回主界面重新登陆",
		callback = function() User:BackToLogin() end,
		noCancelBtn = true
	}
	UIUtil:showPop(PopType.Normal, param)
end

function SocketProxy:errorCommHandle(msg)
	local textCode = msg.code + 10000;
	if msg.code == 1001 then
		self:ShowLostConnection()
	elseif msg.code == 1005 then
			SocketProxy:CloseSocket()
			local param = {
				text = msg.message,
				callback = function() User:BackToLogin() end,
				noCancelBtn = true
			}
			UIUtil:showPop(PopType.Normal, param)
	elseif msg.code == 1006 then
		SocketProxy:CloseSocket()
		local param = {
			text = msg.message,
			callback = function() PlatformHelper.quitGame() end,
			noCancelBtn = true
		}
		UIUtil:showPop(PopType.Normal, param)
	elseif msg.code == 1002 then
		WindowMgr:ShowTips(textCode,true)
		LuaHelper.GetGSNetManager():Disconnect()
	elseif msg.code == 2001 then
		-- 体力不足
		UIUtil:showLackPop(LackPopType.Tili)
	elseif msg.code == 2002 then
		-- 欧元不足
		UIUtil:showLackPop(LackPopType.Coin)
	elseif msg.code == 2003 then
		-- 钻石不足
		UIUtil:showLackPop(LackPopType.Diamond)
	elseif msg.code == 2005 then
		-- 活动已结束
		UIUtil:showTip("活动已结束")
		CommandManager:SendCommand(ActivityMsg.RefreshActivity,"ActivityEnd")
	elseif msg.code == 4141 then
		-- 训练赛挑战次数不足不弹提示
	elseif msg.code == 7055 then
		-- 巅峰对决
		local param = {
			noCancelBtn = true,
			text = "开启区域已满",
		}
		WindowMgr:ShowWindow(WindowNameConst.NormalPop, param,false)
	elseif msg.code == 4115 then
		local lv = User:TeamMgr():getData("playerNumLimitLevel")
		local data = User:GetModel("RoomModel"):getPlayerNum(lv)
		local maxNum = data.player_num_limit
		local param = {
			text = string.format("当前拥有的球员数量已达上限\n（%d/%d），是否前往球员分解？", User:PlayerMgr():getPlayerList():Size(), maxNum),
			callback = function()
				local isOpen, jumpFunc = JumpUtil:HandleJumpFunc(8, 5)
				if jumpFunc then jumpFunc() end
			end,
		}
		UIUtil:showPop(PopType.Normal, param)
	elseif msg.code == 4195 then
		AddictionMgr:ShowHintPop(AddictionPopType.TimeNotPass, msg.message)
	elseif msg.code == 5149 then
		UIUtil:showTip("红包已领完！")
		CommandManager:SendCommand("RefreshRedPackage","PackageNotEnough")
	elseif msg.code == 5151 or msg.code == 5148 then
		WindowMgr:ShowTips(msg.message)
		CommandManager:SendCommand(RedPointMsg.OFF_ROLLNOTICE_HB)
	elseif msg.code == 7015 then
		WindowMgr:ShowTips(msg.message)
		CommandManager:SendCommand("RefreshEggInfo")
	else
		WindowMgr:ShowTips(msg.message)
	end
end

function SocketProxy:ClearHandleProto(opcode)
	table.eraseBy(self.mMsgList,function (v)
		return opcode == v.opcode and not v.registered
	end)
end

function SocketProxy:ClearAllHandleProto()
	self.mLastMsgList = self.mMsgList
	self.mMsgList = {}
	table.foreach(self.mLastMsgList,function (_,v)
		 if v.registered then
			table.insert(self.mMsgList,v)
		 end
	end)
	self:ClearAllHandleError()
end

function SocketProxy:RemoveHandleByOpcode(opcode)
	table.eraseBy(self.mMsgList,function (v)
		return opcode == v.opcode and not v.registered
	end)
end

function SocketProxy:ClearAllHandleError()
	self.lastErrorMsgList = self.errorMsgList
	self.errorMsgList = {}
	-- table.eraseBy(self.errorMsgList,function (v)
	-- 	return not v.registered
	-- end)
end

function SocketProxy:CloseSocket()
	self.activeDisconnect = true -- 主动断开
	LuaHelper.GetGSNetManager():Disconnect();
end

SocketProxy:ctor()