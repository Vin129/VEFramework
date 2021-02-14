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
    using UnityEngine;
    using UnityEditor;
    using LitJson;
    using VEFramework.Editor;

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

        [MenuItem("VETool/LuaKit/AddSymbols")]
        public static void AddSymbols()
        {
            if(ScriptBaseSetting.ToLuaSourceSaveCheck)
            {
                PlayerSettingExtensions.SetDefineSymbols(ScriptBaseSetting.ToLuaDefineSymbol);
            }
            else if(ScriptBaseSetting.XLuaSourceSaveCheck)
            {
                PlayerSettingExtensions.SetDefineSymbols(ScriptBaseSetting.XLuaDefineSymbol);
            }
            
            AssetDatabase.Refresh();
        }

        [MenuItem("VETool/LuaKit/CopyLuaToResource")]
        public static void CopyLuaToResource()
        {
            LuaPacker.CopyLuaFilesToResource();
            AssetDatabase.Refresh();
            Log.IColor("Lua Copy done",LogColor.Orange);
        }
  
        [MenuItem("VETool/LuaKit/Setting")]
        public static void ShowWindow()
        {
            instance = EditorWindow.GetWindow<ScriptKitEditorWindow>();
            hotScriptName = ScriptBaseSetting.GetHotScriptName();
            instance.titleContent = new GUIContent(hotScriptName + "Kit" + ScriptBaseSetting.VERSIONS);
            instance.Init();
            instance.Show();
        }

        [MenuItem ("VETool/VAsset/EasyBuild(ScriptKit)")]
		public static void EasyBuild()
		{
            CopyLuaToResource();
			ABBuilderEditor.AssetBundleEasyBuild();
			Log.IColor("AB Build Over",LogColor.OrangeRed);
		}


        public void Init()
        {
            contents = new Contents();
            guiStyle = new GUIStyle();
            scriptPath = EditorPrefs.GetString(ScriptBaseSetting.KEY_SCRIPT_PATH,string.Empty);
            scriptPathHead = ScriptBaseSetting.NOW_PATH_HEAD;
            scriptPathTail = ScriptBaseSetting.NOW_PATH_TAIL;
            if(scriptPath.Equals(string.Empty))
                scriptPath = scriptPathHead + scriptPathTail;
            EditorPrefs.SetString(ScriptBaseSetting.KEY_SCRIPT_PATH,scriptPath);
        }

        public void OnGUI()
        {
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