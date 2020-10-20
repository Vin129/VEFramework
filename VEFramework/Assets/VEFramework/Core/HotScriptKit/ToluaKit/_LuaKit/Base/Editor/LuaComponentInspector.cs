#define LUA_KIT //没有lua环境请注释
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using EGL = UnityEditor.EditorGUILayout;
using GL = UnityEngine.GUILayout;
using System;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor.SceneManagement;
using VEFramework;
#if LUA_KIT
using LuaInterface;
#endif

namespace LuaKit
{
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

		// 添加TestInspector组件的GameObject被选中时触发该函数
		private void OnEnable()
		{
			if (obj == null || obj.Equals(null))
			{
				obj = new SerializedObject(target);
			}
			if(luaVeiwer == null)
				luaVeiwer = new QuickLuaViewer((msg)=>{UnityEngine.Debug.LogWarning(msg);});
		}

		// 重写Inspector检视面板
		// public override void OnInspectorGUI()
		// {
		// 	base.OnInspectorGUI();

		// 	var path = obj.FindProperty("LuaPath").stringValue;
		// 	path = path ?? "";

		// 	EditorGUILayout.LabelField("LuaPath:");

		// 	var boxRect = EditorGUILayout.GetControlRect();
		// 	boxRect.height *= 1.2f;
		// 	GUI.Box(boxRect, path);
		// 	EditorGUILayout.Space();
		// 	EditorGUILayout.LabelField("请将 Lua 脚本文件拖到下边区域");
		// 	var sfxPathRect = EditorGUILayout.GetControlRect();
		// 	sfxPathRect.height = 200;
		// 	GUI.Box(sfxPathRect, string.Empty);
		// 	EditorGUILayout.LabelField(string.Empty, GUILayout.Height(185));
		// 	if (
		// 		Event.current.type == EventType.DragUpdated
		// 		&& sfxPathRect.Contains(Event.current.mousePosition)
		// 	)
		// 	{
		// 		//改变鼠标的外表  
		// 		DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
		// 		if (DragAndDrop.paths != null && DragAndDrop.paths.Length > 0)
		// 		{
		// 			if (DragAndDrop.paths[0] != "")
		// 			{
		// 				var newPath = DragAndDrop.paths[0];
		// 				var resultString = Regex.Split(newPath, "/ToLua/", RegexOptions.IgnoreCase);
		// 				newPath = resultString[1];

		// 				newPath = newPath.Replace(".lua", "");
		// 				newPath = newPath.Replace("/", ".");

		// 				obj.FindProperty("LuaPath").stringValue = newPath;
		// 				obj.FindProperty("LuaFilePath").stringValue = DragAndDrop.paths[0];
		// 				obj.ApplyModifiedPropertiesWithoutUndo();
		// 				AssetDatabase.SaveAssets();
		// 				AssetDatabase.Refresh();
		// 				EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
		// 			}
		// 		}
		// 	}

	
		// }
		private string focusFilePath;
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
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
							var resultString = Regex.Split(newPath, "/ToLua/", RegexOptions.IgnoreCase);
							newPath = resultString[1];

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