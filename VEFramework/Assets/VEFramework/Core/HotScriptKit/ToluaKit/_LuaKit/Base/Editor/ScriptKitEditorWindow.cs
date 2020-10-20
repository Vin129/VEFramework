/****************************************************************************
 * Copyright (c) 2019.7 vin129
 ****************************************************************************/

namespace LuaKit
{
    using UnityEngine;
    using UnityEditor;
    using LitJson;
    public class ScriptKitEditorWindow:EditorWindow
    {
        private static ScriptKitEditorWindow instance;
        private static Contents contents;
        private static string hotScriptName;
        private GUIStyle guiStyle;

        private Vector2 scrollPos;
        private string scriptPathHead;
        private string scriptPathTail;
        private string scriptPath;
  
        [MenuItem("VETool/LuaKit/Setting")]
        public static void ShowWindow()
        {
            instance = EditorWindow.GetWindow<ScriptKitEditorWindow>();
            hotScriptName = ScriptBaseSetting.GetHotScriptName();
            instance.titleContent = new GUIContent(hotScriptName + "Kit" + ScriptBaseSetting.VERSIONS);
            instance.Init();
            instance.Show();
        }

        public void Init(){
            contents = new Contents();
            guiStyle = new GUIStyle();
            scriptPath = EditorPrefs.GetString(ScriptBaseSetting.KEY_SCRIPT_PATH,string.Empty);
            scriptPathHead = ScriptBaseSetting.NOW_PATH_HEAD;
            scriptPathTail = ScriptBaseSetting.NOW_PATH_TAIL;
            if(scriptPath.Equals(string.Empty))
                scriptPath = scriptPathHead + scriptPathTail;
            EditorPrefs.SetString(ScriptBaseSetting.KEY_SCRIPT_PATH,scriptPath);
        }

        public void OnGUI(){
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
			GUILayout.BeginVertical();
            GUILayout.Label(string.Format("{0}CodePath:{1}",hotScriptName,scriptPath));
            GUILayout.BeginHorizontal();
			GUILayout.Label(contents.ScriptPathHeadContent);
            GUILayout.TextField(scriptPathHead);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(contents.ScriptPathTailConten);
            scriptPathTail = GUILayout.TextField(scriptPathTail);
            GUILayout.EndHorizontal();
	        if (GUILayout.Button("Clear Setting"))
			{
				EditorPrefs.DeleteKey(ScriptBaseSetting.KEY_SCRIPT_PATH);
			}
            if (GUILayout.Button("Save Setting"))
			{
               SaveData();
			}

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        private void OnEnable()
		{

        }

        private void OnDisable()
        {

        }

        private void SaveData(){
            scriptPath = scriptPathHead + scriptPathTail;
            EditorPrefs.SetString(ScriptBaseSetting.KEY_SCRIPT_PATH,scriptPath);
            ScriptBaseSetting.SetBaseData(ScriptBaseSetting.KEY_SCRIPT_PATH_TAIl,scriptPathTail);
            ScriptBaseSetting.SetBaseData(ScriptBaseSetting.KEY_SCRIPT_PATH,scriptPath);
            ScriptBaseSetting.SaveBaseData();
            AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
            Debug.Log("<color=#EE6A50>Save Data Sucess!</color>");
        }

         private class Contents
		{
			public readonly GUIContent ScriptPathHeadContent = new GUIContent("ScriptPathHead:",
				"Based on different scripts");
            public readonly GUIContent ScriptPathTailConten  = new GUIContent("ScriptPathTail:",
                "CodeGenerator Path");
		}
    }
}