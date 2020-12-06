
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
namespace VEFramework
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using EGL = UnityEditor.EditorGUILayout;
	using GL = UnityEngine.GUILayout;
	using System;
	using System.Linq;
	public class QuickExecuteEditor : EditorWindow 
	{
		[Flags]
		enum QEType
		{
			None,
			CSharp,
			Lua,
		}
		#if LUA_KIT
			QuickLuaViewer luaVeiwer;
			public void InitLuaVeiwer()
			{
				if(luaVeiwer == null)
				{
					luaVeiwer = new QuickLuaViewer(ClearFocusFile);
				}
			}
		#endif
		[MenuItem("VETool/Utility/QuickExecute %&Q")]
		private static void ShowWindow() {
			var window = GetWindow<QuickExecuteEditor>();
			#if LUA_KIT
				window.InitLuaVeiwer();
			#endif
			window.minSize = new Vector2(700,300);
			window.titleContent = new GUIContent("QuickExecute");
			window.Show();
		}

		private bool searchScript = false;
		private void OnGUI() {
			GUI.skin.label.richText = true;

			if(GL.Button("Search Can Execute Script"))
			{
				searchScript = true;
				DoSearch();
			}
			if(searchScript == true)
				DrawSearchResult();
			if(focusFileName.IsNullOrEmpty())
			{
				DrawDragSpace();
			}
			else
			{
				GL.Space(5);
				GL.BeginVertical("box");
				GL.Label(string.Format("FocusFileName : {0}",focusFileName),EditorStyles.boldLabel);
				GL.Label(string.Format("FocusFileType : {0}",focusFileType.ToString()),EditorStyles.boldLabel);
				GL.Label(string.Format("FocusFilePath : {0}",focusFilePath),EditorStyles.boldLabel);
				focusFileRect = GL.BeginScrollView(focusFileRect,"box");
				if(focusFileType == QEType.CSharp)
					DrawScript(GetFocusFileType(focusFileName));
				if(focusFileType == QEType.Lua)
				{
					#if LUA_KIT
						luaVeiwer.DrawLuaScript(focusFilePath);
					#else
						EGL.HelpBox("没有LUA_KIT环境，请查看说明",MessageType.Warning);
					#endif
				}
				if(GL.Button("ClearFocus"))
				{
					ClearFocusFile();
				}
				GL.EndScrollView();
				GL.EndVertical();
			}
		}

		private Vector2 focusFileRect;
		private string focusFileName = string.Empty;
		private string focusFilePath = string.Empty;
		private	QEType focusFileType = QEType.None;
		private void DrawDragSpace(){
			EditorGUILayout.LabelField("请将脚本文件拖到下边区域");
			var sfxPathRect = EditorGUILayout.GetControlRect(GUILayout.Height(100));
			GUI.Box(sfxPathRect, string.Empty);
			// EditorGUILayout.LabelField(string.Empty, GUILayout.Height(185));
			if (Event.current.type == EventType.DragUpdated&& sfxPathRect.Contains(Event.current.mousePosition))
			{
				DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
			}
			if(Event.current.type == EventType.DragPerform && sfxPathRect.Contains(Event.current.mousePosition))
			{
				string path = null;
				if(DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
					path = DragAndDrop.paths[0];
				if(DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
				{
					if(path != null && path.EndsWith(".cs") || path.EndsWith(".lua"))
					{
						focusFileName = DragAndDrop.objectReferences[0].name;
						focusFilePath = path;
					}
				}
				if(focusFileName.IsNullOrEmpty())
				{
					Log.W("Incorrect file");
					ShowNotification(new GUIContent("Incorrect file (Nonconforming .cs or .lua)"));
				}
				else
				{
					if(path.EndsWith(".cs"))
					{
						focusFileType = QEType.CSharp;
						if(GetFocusFileType(focusFileName) == null)
						{
							var msg = "File Name : <color=#FF0000>{0}</color>\nmake sure .cs file is correct (name same as TypeName)";
							Log.W(string.Format(msg,focusFileName));
							ShowNotification(new GUIContent(string.Format("Incorrect  {0}",focusFileName)));
							ClearFocusFile();
						}
					}
					if(path.EndsWith(".lua"))
						focusFileType = QEType.Lua;
				}
			}
		}

		private void ClearFocusFile(string errorMsg = null)
		{
			if(!errorMsg.IsNullOrEmpty())
			{
				Log.W(errorMsg);
				ShowNotification(new GUIContent(errorMsg));
			}
			focusFileName = string.Empty;
			focusFilePath = string.Empty;
			focusFileType = QEType.None;
			#if LUA_KIT
				luaVeiwer.ClearLuaState();
			#endif	
		}	

		private Type GetFocusFileType(string name)
		{
			var types = ReflectionTools.GetAllTypes(false).Where((t)=>{
				var att = t.RTGetAttribute<QuickExecuteAttribute>(false);
				return(att != null && att.CanSearch && t.Name.Equals(name));
			});
			if(types.Count() < 1)
				return null;
			return types.ToArray()[0];
		}

		private List<Type> searchTypeList;
		private void DoSearch()
		{
			searchTypeList = new List<Type>();
			var types = ReflectionTools.GetAllTypes(false).Where((t)=>{
				var att = t.RTGetAttribute<QuickExecuteAttribute>(false);
				return(att != null && att.CanSearch);
			});
			searchTypeList = types.ToList();
		}
		private Vector2 scrollRect;
		private void DrawSearchResult()
		{
			if(searchTypeList == null || searchTypeList.Count < 1)
				return;
			scrollRect = GL.BeginScrollView(scrollRect,"box");
			searchTypeList.ForEach((t)=>{
				DrawScript(t);
				GL.Space(5);
			});
			GL.EndScrollView();
		}
		private void DrawScript(Type t)
		{
			var att = t.RTGetAttribute<QuickExecuteAttribute>(false);
			if(att == null)
				return;
			GL.BeginVertical("OL box");
			GL.Label(t.Name,EditorStyles.boldLabel);
			var methods  = t.GetMethods().Where((i)=>{return i.RTGetAttribute<ExecuteMethodAttribute>(false) != null;});
			if(methods.Count() < 1)
			{
				EGL.HelpBox("没有可以被执行的方法(请检查是否添加了[ExecuteMethodAttribute])",MessageType.Warning);
			}
			else
			{
				foreach (var m in methods)
				{
					GL.Space(2);
					GL.BeginHorizontal();
					var methodName = m.Name + ":(";
					var param = m.GetParameters();
					for(int i = 0;i<param.Length;i++)
					{
						var p = param[i];
						if(i != param.Length - 1)
							methodName += string.Format("<b>{0}</b> , ",p.ToString());
						else
							methodName += string.Format("<b>{0}</b>",p.ToString());				
					}
					methodName += ")";
					GL.Label(methodName);
					if(GL.Button("Execute",GUILayout.Width(100)))
					{
						var o = ReflectionTools.CreateObject(t);
						//TODO 扩展编辑参数	
						if(param.Length == 0)
							m.Invoke(o,null);
						Log.I(string.Format("<color=#FFA80B>Execute {0}</color>",methodName));
					}
					GL.EndHorizontal();
					GL.Space(2);
				}
			}
			GL.EndVertical();
		}
		
		private void OnProjectChange() {
			ClearFocusFile();
		}
		private void OnDestroy() {
			ClearFocusFile();
		}
	} 
}
