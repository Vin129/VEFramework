﻿/****************************************************************************
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


namespace  VEFramework
{
	using System.Collections.Generic;
	using UnityEngine;
	using System;
	using System.Text.RegularExpressions;
	using System.Linq;
	using UnityEditor;
	using EGL = UnityEditor.EditorGUILayout;
	using GL = UnityEngine.GUILayout;
	using VEFramework.HotScriptKit;

#if DEFINE_VE_TOLUA
	using LuaInterface;
	public class QuickLuaViewer
	{
		private Action<string> debugger;
		public QuickLuaViewer(Action<string> action)
		{
			debugger = action;
		}
		private void Report(string msg)
		{
			if(debugger == null)
			{
				Log.W(msg);
				return;
			}
			debugger.Invoke(msg);
		}

		public void DrawLuaScript(string filePath)
		{
			FindLuaTableInfo(filePath);
			// 以上是初始化环节
			var luaName = filePath.Substring(filePath.LastIndexOf("/") + 1,filePath.Length - filePath.LastIndexOf("/") - 1).Replace(".lua","");
			GL.BeginVertical("OL box");
			GL.Label(luaName,EditorStyles.boldLabel);
			if(mExecuteFunctionDirt == null || mExecuteFunctionDirt.Count() < 1)
			{
				EGL.HelpBox("没有可以被执行的方法",MessageType.Warning);
			}
			else
			{
				foreach (var m in mExecuteFunctionDirt)
				{
					GL.BeginHorizontal("box");
					var methodName = string.Format("{0}:{1}",luaName,m.Key);
					GL.Label(methodName);
					if(GL.Button("Execute",GUILayout.Width(100)))
					{	
						UnityEngine.Debug.LogFormat("<color=#FFA80B>Execute {0}:{1}</color>",luaName,m.Key);
						m.Value.Invoke();
					}
					GL.EndHorizontal();
					GL.Space(2);
				}
			}
			GL.EndVertical();
		}

		public void ClearLuaState()
		{
			if(mExecuteFunctionDirt != null)
				mExecuteFunctionDirt.Clear();
			if(mFocusLuaTab != null)
				mFocusLuaTab.Dispose();
			if(mLuaState != null)
				mLuaState.Dispose();

			mExecuteFunctionDirt = null;
			mFocusLuaTab = null;
			mLuaState = null;
		}


		private string LuaDefaultFilePath  = "Framework/init.lua";
		private readonly string CreateLuaFile = "CreateLuaFile";
		private readonly string[] IgonreBaseName = {"new","ctor","create"};
		private LuaState mLuaState;
		private LuaTable mFocusLuaTab;
		private Dictionary<string,Action> mExecuteFunctionDirt;
		private void LuaStateInit()
		{
			#if UNITY_EDITOR
				LuaFileUtils.Instance.beZip = false;
			#else
				LuaFileUtils.Instance.beZip = true;
			#endif
			mLuaState = new LuaState();
			mLuaState.LogGC = false;
			mLuaState.Start();
			LuaBinder.Bind(mLuaState);
			mLuaState.DoFile(LuaDefaultFilePath);		
		}

		private void FindLuaTableInfo(string filePath)
		{
			if(mLuaState == null)
				LuaStateInit();
			if(mLuaState == null)
			{
				Report("LuaKitError:LuaState init fail");
				return;
			}
			if(mFocusLuaTab == null)
			{
				var resultString = Regex.Split(filePath,ScriptBaseSetting.LuaGamePath.Replace(Application.dataPath,"") + "/", RegexOptions.IgnoreCase);
				if(resultString.Length < 2)
				{
					var msg = "PathError: Not Found {0}/...";
					Report(string.Format(msg,ScriptBaseSetting.LuaGamePath));
					return;
				}
				filePath = resultString[1];	
				Debug.LogError(filePath);
				LuaFunction func = mLuaState.GetFunction(CreateLuaFile);
				if (null == func)
				{
					var msg = "LuaKitError: Not Found Function {0}";
					Report(string.Format(msg,CreateLuaFile));
					return;
				}
				mFocusLuaTab = func.Invoke<string,LuaTable>(filePath.Replace(".lua",""));
			}
			if(mFocusLuaTab == null)
			{
				Report("LuaKitError:LuaTable init fail");
				return;
			}
			if(mFocusLuaTab["class"].IsNull())
			{
				Report("LuaKitError:LuaTable init not a class");
				return;
			}
			if(mExecuteFunctionDirt == null)
			{
				mExecuteFunctionDirt = new Dictionary<string, Action>();
				foreach (var item in ((LuaTable)mFocusLuaTab["class"]).ToDictTable())
				{
					if(item.Value.GetType() == typeof(LuaInterface.LuaFunction))
					{
						if(!IgonreBaseName.Contains(item.Key.ToString()))
						{
							var key = item.Key.ToString();
							mExecuteFunctionDirt.Add(key,()=>{
								var luafunc = mFocusLuaTab.GetLuaFunction(key);
								if(luafunc != null)
								{
									luafunc.Call<LuaTable>(mFocusLuaTab);
								}
							});
						}
					}
				}
			}	
		}
	}


#elif DEFINE_VE_XLUA
	using XLua;
	public class QuickLuaViewer
	{
		private Action<string> debugger;
		public QuickLuaViewer(Action<string> action)
		{
			debugger = action;
		}
		private void Report(string msg)
		{
			if(debugger == null)
			{
				Log.W(msg);
				return;
			}
			debugger.Invoke(msg);
		}

		public void DrawLuaScript(string filePath)
		{
			FindLuaTableInfo(filePath);
			// 以上是初始化环节
			var luaName = filePath.Substring(filePath.LastIndexOf("/") + 1,filePath.Length - filePath.LastIndexOf("/") - 1).Replace(".lua","");
			GL.BeginVertical("OL box");
			GL.Label(luaName,EditorStyles.boldLabel);
			if(mExecuteFunctionDirt == null || mExecuteFunctionDirt.Count() < 1)
			{
				EGL.HelpBox("没有可以被执行的方法",MessageType.Warning);
			}
			else
			{
				foreach (var m in mExecuteFunctionDirt)
				{
					GL.BeginHorizontal("box");
					var methodName = string.Format("{0}:{1}",luaName,m.Key);
					GL.Label(methodName);
					if(GL.Button("Execute",GUILayout.Width(100)))
					{	
						UnityEngine.Debug.LogFormat("<color=#FFA80B>Execute {0}:{1}</color>",luaName,m.Key);
						m.Value.Invoke();
					}
					GL.EndHorizontal();
					GL.Space(2);
				}
			}
			GL.EndVertical();
		}

		public void ClearLuaState()
		{
			if(mExecuteFunctionDirt != null)
				mExecuteFunctionDirt.Clear();
			if(mFocusLuaTab != null)
				mFocusLuaTab.Dispose();
			if(mLuaEnv != null)
				mLuaEnv.Dispose();

			mExecuteFunctionDirt = null;
			mFocusLuaTab = null;
			mLuaEnv = null;
		}


		private string LuaDefaultFilePath  = "Framework/init.lua";
		private readonly string CreateLuaFile = "CreateLuaFile";
		private readonly string[] IgonreBaseName = {"new","ctor","create"};
		private LuaEnv mLuaEnv;
		private LuaTable mFocusLuaTab;
		private Dictionary<string,Action> mExecuteFunctionDirt;
		private void LuaStateInit()
		{

			mLuaEnv = new LuaEnv();
			mLuaEnv.AddLoader(VAsset.Instance.XLuaLoader);
			mLuaEnv.DoString("require",LuaDefaultFilePath);		
		}

		private void FindLuaTableInfo(string filePath)
		{
			if(mLuaEnv == null)
				LuaStateInit();
			if(mLuaEnv == null)
			{
				Report("LuaKitError:LuaState init fail");
				return;
			}
			if(mFocusLuaTab == null)
			{
				var resultString = Regex.Split(filePath,ScriptBaseSetting.LuaGamePath.Replace(Application.dataPath,"") + "/", RegexOptions.IgnoreCase);
				if(resultString.Length < 2)
				{
					var msg = "PathError: Not Found {0}/...";
					Report(string.Format(msg,ScriptBaseSetting.LuaGamePath));
					return;
				}
				filePath = resultString[1];	
				Debug.LogError(filePath);
				LuaFunction func = mLuaEnv.Global.Get<LuaFunction>(CreateLuaFile);
				if (null == func)
				{
					var msg = "LuaKitError: Not Found Function {0}";
					Report(string.Format(msg,CreateLuaFile));
					return;
				}
				mFocusLuaTab = func.Func<string,LuaTable>(filePath.Replace(".lua",""));
			}
			if(mFocusLuaTab == null)
			{
				Report("LuaKitError:LuaTable init fail");
				return;
			}
			if(mFocusLuaTab.Get<LuaTable>("class").IsNull())
			{
				Report("LuaKitError:LuaTable init not a class");
				return;
			}
			if(mExecuteFunctionDirt == null)
			{
				mExecuteFunctionDirt = new Dictionary<string, Action>();
				mFocusLuaTab.Get<LuaTable>("class").ForEach<LuaFunction,string>((f,name)=>{
					if(!IgonreBaseName.Contains(name))
					{
						var key = name;
						mExecuteFunctionDirt.Add(key,()=>{
							var luafunc = mFocusLuaTab.Get<LuaFunction>(key);
							if(luafunc != null)
							{
								luafunc.Call(mFocusLuaTab);
							}
						});
					}
				});
			}	
		}
	}
#endif
}


