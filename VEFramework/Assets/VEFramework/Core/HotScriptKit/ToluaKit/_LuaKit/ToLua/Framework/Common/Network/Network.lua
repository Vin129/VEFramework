module("Network",package.seeall)


local Network = {}
Network.m_msgID = 0 --自动计数
Network.m_HaveConnect = false
Network.m_ConnectCb = nil;
-- function Network:ctor()
        
-- end
function Network:OnRecvMsg(kMsg)
   CommandManager:SendCommand(kMsg["ID"],kMsg)
end

function Network:Connect(strAddr,callback)
    self.m_ConnectCb = callback
    LuaHelper.Connect(strAddr)
end

--发送socket请求,type 服务器路由,obj 协议数据
function Network:SendMsg(type,obj)
    if not self.m_HaveConnect then
        log("error:和websocket服务器处于断开状态")
        return
    end
    self.m_msgID = self.m_msgID + 1
    local param = {
        id = self.m_msgID,
        type = type,
        data = obj or {};
    }

    local body = json.encode(param)
    log("socket reuqest:"..body)
    LuaHelper.SendMessage(body)
end

--发送起http请求
function Network:Request(url, obj, callback)
    WindowMgr:ShowWaiting(function() self:RestNetworkHandler() end,true)

    obj = obj or {};
    if not obj.uid then
        obj.uid = ServerDef.U_SERVER_ID.."-"..ServerDef.U_ID;
    end
    if not obj.token then
        obj.token = ServerDef.U_TOKEN;
    end

	self.m_msgID = self.m_msgID + 1;
	local params = {
		ts = self.m_msgID,
		data = obj
	}
	
	local body = json.encode(params);
	
	log("request:" .. url .. "," .. body)
	LuaHelper.DoPost(url, body, function(error, jsomMsg)
		log("respose：" .. jsomMsg);
		local output = json.decode(jsomMsg);
		if(callback ~= nil) then
			callback(output)
		end
		WindowMgr:HideWaiting(true)
	end)
end

function Network:RestNetworkHandler()
    LuaHelper.RestNetworkHandler()
end

function Network:OnHttpRequestCb(wwwError,jsonMsg)
    local kTbl = json.decode(jsonMsg);
    if nil ~= wwwError and 0 ~= string.len(wwwError) then
        -- process error
        return;
    end

    CommandManager:SendCommand(kTbl['msgid'],kTbl);
end

function Network:OnNoHttpRequestCb()
    if nil ~= self.mWaitingView then
      --  WindowMgr:CloseWindow(self.mWaitingView,true)
        self.mWaitingView = nil;
    end
end

function Network.OnWebSocketCb(msgType,jsonMsg)
    log("OnWebSocketCb:"..jsonMsg)
    if msgType == "Error" then
        log("OnWebSocketCb:Error");
        CommandManager:SendCommand(msgType, jsonMsg);
    elseif msgType == "Close" then
        log("OnWebSocketCb:Closed");
        Network.m_HaveConnect = false
        CommandManager:SendCommand(msgType, jsonMsg);
    elseif msgType == "Connect" then
        Network.m_HaveConnect = true
        log("OnWebSocketCb:Connected");
        if Network.m_ConnectCb then
            Network.m_ConnectCb();
            Network.m_ConnectCb = nil;
        end
	
	CommandManager:SendCommand(msgType, jsonMsg);        
    elseif msgType == "Message" then
        log("OnWebSocketCb:Message:");       
        local resp = json.decode(jsonMsg)
        local type = resp.type; 
        CommandManager:SendCommand(type, resp);
    end
end

--protobuf
--[[ 
function Network:OnWebSocketCb(msgid,msgType,msgDesc,rawdata)
    log("OnWebSocketCb")
    if msgType == "Error" then
    elseif msgType == "Open" then
        -- 处理open信息
    elseif msgType == "Close" then
        -- 处理关闭信息
    elseif msgType == "Message" then
       -- 处理rawdata
       CommandManager:SendCommand(msgid,rawdata)
    end
end
--]]

return Network
