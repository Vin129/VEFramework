FRAMEWORK_INITED = false

if not FRAMEWORK_INITED then
	require ("Framework/Function")
	-- require("Framework/Common/LuaExtension")
	require ("Framework/Define")
	require ("Framework/Utility")
	require ("Framework/MsgDispatcher")
	require ("Framework/FSM")	
	-- require ("Framework/Logic/Command/ICommand") -- 暂时先不处理消息
	require ("Framework/LuaBehaviour")

	require ("Framework/LuaDebug")

	function Main() 

	end

	function OnLevelWasLoaded(level)
		
	end

	--创建lua文件
	function CreateLuaFile(luaFilePath,gameObject)
		log("CreateLuaFile:"..luaFilePath)
		local luaTable = nil
		luaTable = require(luaFilePath).new()
		return luaTable
	end

	-- function ReadLuaFile(luaFilePath)
	-- 	log("ReadLuaFile:"..luaFilePath)
	-- 	local luaTable = nil
	-- 	luaTable = dofile(luaFilePath).new()
	-- 	return luaTable
	-- end

	FRAMEWORK_INITED = true

	log("Init Sucess")
end 