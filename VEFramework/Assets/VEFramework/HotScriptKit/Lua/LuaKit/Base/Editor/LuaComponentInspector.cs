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

#if DEFINE_VE_LUA
namespace VEFramework.HotScriptKit
{
	using UnityEngine;
	using UnityEditor;
	using System.Text.RegularExpressions;
	using UnityEditor.SceneManagement;
	using VEFramework;
	using LuaInterface;
	[CustomEditor(typeof(LuaComponent))]
	public class LuaComponentInspectorEditor : UnityEditor.Editor
	{
		QuickLuaViewer luaVeiwer;
		public void InitLuaVeiwer()
		{
			if(luaVeiwer == null)
			{
				
			}
		}
		private SerializedObject obj;
		private void OnEnable()
		{
			if (obj == null || obj.Equals(null))
			{
				obj = new SerializedObject(target);
			}
			if(luaVeiwer == null)
				luaVeiwer = new QuickLuaViewer((msg)=>{Log.E(msg);});
		}

		private void OnDisable() 
		{
			if(luaVeiwer != null)
				luaVeiwer.ClearLuaState();
		}

		private string focusFilePath;
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			UpdateInspector();
		}

		
		public void UpdateInspector()
		{
			var path = obj.FindProperty("LuaFilePath").stringValue;
			focusFilePath = path ?? string.Empty;
			if(focusFilePath.IsNullOrEmpty())
			{
				EditorGUILayout.LabelField("请将脚本文件拖到下边区域");
				var sfxPathRect = EditorGUILayout.GetControlRect(GUILayout.Height(100));
				GUI.Box(sfxPathRect, string.Empty);
				if (Event.current.type == EventType.DragUpdated&& sfxPathRect.Contains(Event.current.mousePosition))
				{
					DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
				}
				if(Event.current.type == EventType.DragPerform && sfxPathRect.Contains(Event.current.mousePosition))
				{
					path = null;
					if(DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
						path = DragAndDrop.paths[0];
					if(DragAndDrop.objectReferences != null && DragAndDrop.objectReferences.Length > 0)
					{
						if(path != null && path.EndsWith(".lua"))
						{
							var newPath = DragAndDrop.paths[0];
							if(newPath.StartsWith(ScriptBaseSetting.LuaGamePath))
							{
								EditorUtility.DisplayDialog("提示", "Lua脚本路径错误，请查看ScriptBaseSetting并重新配置", "确定");
								return;
							}
							newPath = newPath.Replace(ScriptBaseSetting.LuaGamePath,"");
							newPath = newPath.Replace(".lua", "");
							newPath = newPath.Replace("/", ".");

							obj.FindProperty("LuaPath").stringValue = newPath;
							obj.FindProperty("LuaFilePath").stringValue = DragAndDrop.paths[0];
							obj.ApplyModifiedPropertiesWithoutUndo();
							AssetDatabase.SaveAssets();
							AssetDatabase.Refresh();
							EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
						}
					}
				}
			}
			else
			{
				luaVeiwer.DrawLuaScript(focusFilePath);
				if (!obj.FindProperty("LuaFilePath").stringValue.IsNullOrEmpty())
				{
					if (GUILayout.Button("刷新脚本"))
					{
						focusFilePath = null;
						luaVeiwer.ClearLuaState();
						AssetDatabase.Refresh();
					}

					if (GUILayout.Button("选择脚本"))
					{
						Selection.activeObject =
							AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(obj.FindProperty("LuaFilePath").stringValue);
					}
				}
			}
		}


	}
}
#endif