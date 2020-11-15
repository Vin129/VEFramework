FRAMEWORK_INITED = false

if not FRAMEWORK_INITED then
	require ("Framework/Function")
	require ("Framework/Common/LuaExtension")
	require ("Framework/Define")
	require ("Framework/Utility")
	require ("Framework/FSM")	
	require ("Framework/Common/TimerMgr")	
	require ("Framework/Logic/Command/ICommand")
	require ("Framework/LuaBehaviour")
	require ("Framework/LuaDebug")

	UIManager = require("Framework/Logic/UI/UIManager")
	CommandManager = require("Framework/Logic/Command/CommandManager")
	

	function Main() 
		log("Star Main")
	end

	--创建lua文件
	function CreateLuaFile(luaFilePath,gameObject)
		log("CreateLuaFile:"..luaFilePath)
		local luaTable = nil
		luaTable = require(luaFilePath).new()
		return luaTable
	end

	function MonoUpdate(DeltaTime)
		if nil ~= TimerMgr then
			TimerMgr:Update(DeltaTime);
		end
		CommandManager:Update()
	end

	FRAMEWORK_INITED = true

	Main()
end 