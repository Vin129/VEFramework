/****************************************************************************
 * Copyright (c) 2020 vin129
 *  
 * May the Force be with you :)
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 ****************************************************************************/
namespace VEFramework.HotScriptKit
{
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
		public static bool SourceSaveCheck 
		{
			get
			{
				return ToLuaSourceSaveCheck | XLuaSourceSaveCheck;
			}
		}

		public static bool ToLuaSourceSaveCheck
		{
			get
			{
				return Directory.Exists(LuaSourcePath + "/_Tolua");
			}
		}

		public static bool XLuaSourceSaveCheck
		{
			get
			{
				return Directory.Exists(LuaSourcePath + "/_Xlua");
			}
		}

		public static string ToLuaDefineSymbol = "DEFINE_VE_TOLUA";
		public static string XLuaDefineSymbol = "DEFINE_VE_XLUA";

		// Lua 来源路径 （Tolua&Xlua...）
		public static string LuaSourcePath = Application.dataPath + "/VEFramework/HotScriptKit/Lua/Source";
		// Lua Game
		public static string LuaGamePath = LuaSourcePath + "/_Game";
		// LuaKit 路径
		public static string LuaKitPath = Application.dataPath + "/VEFramework/HotScriptKit/Lua/LuaKit";

		// TODO:都将支持窗口设置
		public static string VERSIONS = "0.1.1";
		public static string SETTING_DATA_PATH = LuaKitPath + "/ScriptKitData/BaseSetting.json";
#region  EditorKey 
		public static string KEY_SCRIPT_PATH_TAIl = "SCRIPT_PATH_TAIL";
		public static string KEY_SCRIPT_PATH = "SCRIPT_PATH";
#endregion

#if UNITY_EDITOR
		public static string ScriptPathTail = "/UI";
#else 
		public static string ScriptPathTail = "/UI";
#endif
#region ToLua	
		//lua逻辑代码目录
		public static string LUA_DIR = LuaGamePath;     
		//tolua lua文件目录           
		public static string TOLUA_DIR = LuaSourcePath + "/_Tolua/ToLua/Lua";       
#endregion
#region SettingSupprot	
		public static string NOW_PATH_HEAD = LuaGamePath;
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
		public static void SetBaseData(string key,string value)
		{
			BaseData[key] = value;
		}
		public static void SaveBaseData()
		{
			BaseData.SaveJsonData(SETTING_DATA_PATH);
		}

		public static string GetHotScriptName()
		{
			if(ToLuaSourceSaveCheck)
			{
				return "ToLua";
			}
			else if(XLuaSourceSaveCheck)
			{
				return "XLua";
			}
			return "Null";
		}
	}
}
