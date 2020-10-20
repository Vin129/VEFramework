/****************************************************************************
 * Copyright (c) 2019 vin129
 ****************************************************************************/


namespace LuaKit {
	using System;
	using System.IO;
	using UnityEngine;
	using LitJson;

#if UNITY_EDITOR
	using UnityEditor;
    using VEFramework;
#endif
    [Serializable]
	public class ScriptBaseSetting
	{
		public static string ToLuaKitPath = "/VEFramework/Core/HotScriptKit/ToluaKit/";


		// TODO:都将支持窗口设置
		public static string VERSIONS = "0.1.1";

		public static string SETTING_DATA_PATH = Application.dataPath + ToLuaKitPath + "_LuaKit/ScriptKitData/BaseSetting.json";
#region  EditorKey 
		public static string KEY_SCRIPT_PATH_TAIl = "SCRIPT_PATH_TAIL";
		public static string KEY_SCRIPT_PATH = "SCRIPT_PATH";
#endregion

#if UNITY_EDITOR
		public static string ScriptPathTail = "/Game/UI";
#else 
		public static string ScriptPathTail = "/Game/UI";
#endif
		// public static Dictionary<string,string> ;
		public static int NOW_SCRIPT_TYPE = 1;	
#region ToLua	
		public static int LUA_BASE_TYPE = 1;
		public static string LUA_DIR = Application.dataPath + ToLuaKitPath + "_LuaKit/ToLua/Lua";                //lua逻辑代码目录
		public static string TOLUA_DIR = Application.dataPath + ToLuaKitPath + "_LuaKit/ToLua/Lua";        //tolua lua文件目录
#endregion
#region SettingSupprot	
		public static string[] ScriptPathHeads = {String.Empty,LUA_DIR};
		public static string NOW_PATH_HEAD = ScriptPathHeads[NOW_SCRIPT_TYPE] ?? String.Empty;
		public static string NOW_PATH_TAIL {
			get{
				if(!BaseData.Keys.Contains(KEY_SCRIPT_PATH_TAIl)){
					return ScriptPathTail;
				}	
				return BaseData[KEY_SCRIPT_PATH_TAIl].ToString();
			}
			set{
				BaseData[KEY_SCRIPT_PATH_TAIl] = value;
			}
		}
#endregion


		private static JsonData baseData;
		public static JsonData BaseData{
			get{
				if(baseData == null)
				{
					if(File.Exists(SETTING_DATA_PATH)){
						StreamReader reader = new StreamReader(SETTING_DATA_PATH);
						string jsonText = reader.ReadToEnd();
						reader.Close();
						baseData = JsonMapper.ToObject(jsonText);
					}else{
						baseData = new JsonData();
					}	
				}
				return baseData;
			}
			set{
				baseData = value;
			}
		}
		public static void SetBaseData(string key,string value){
			BaseData[key] = value;
		}
		public static void SaveBaseData(){
			BaseData.SaveJsonData(SETTING_DATA_PATH);
		}

		public static string GetScriptPath(int scriptType){
			if(!BaseData.Keys.Contains(KEY_SCRIPT_PATH))
			{
				if(ScriptPathHeads[scriptType] == null)
					return ScriptPathTail;
				return ScriptPathHeads[scriptType] + ScriptPathTail;
			}
			else
			{
				return BaseData[KEY_SCRIPT_PATH].ToString();
			}
		}

		public static string GetHotScriptName(){
			switch(NOW_SCRIPT_TYPE){
				case 1:
					return "ToLua";
				default:
					return "无";
			}
		}
	}
}
