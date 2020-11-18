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
namespace VEFramework.Editor
{
    using System.Collections.Generic;
    using System.IO;
    using LitJson;
    using UnityEditor;
    using UnityEngine;
	using EGL = UnityEditor.EditorGUILayout;
	using GL = UnityEngine.GUILayout;

    public class VEPackageEditor:EditorWindow
	{
		private static string PackagesLocalPath = Application.dataPath + "/VEFramework/VEPackages/Packages/";
		private static string PackageFilePath = Application.dataPath + "/VEFramework/VEPackages/Editor/Data/PackageDatas.json";
        private static JsonData baseData;
		public static JsonData BaseData{
			get{
				if(baseData == null)
				{
					if(File.Exists(PackageFilePath)){
						StreamReader reader = new StreamReader(PackageFilePath);
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

		[MenuItem ("VETool/PackageManager",false,0)]
        public static void ShowWindow()
        {
         	EditorWindow.GetWindow<VEPackageEditor>();
        }

		private Vector2 scrollRect;
		private void OnGUI() 
		{
			if(!File.Exists(PackageFilePath))
			{
				EGL.HelpBox("暂无数据",MessageType.Warning);
                return;
			}

			GUI.skin.label.richText = true;
			scrollRect = GL.BeginScrollView(scrollRect,"box");
			DrawDatas();
			GL.EndScrollView();
		}
		
		private void DrawDatas()
		{
			var packages = BaseData["Packages"];
			if( packages.Count > 0 )
			{
				for(int i = 0;i < packages.Count;i++)
				{
					var value = packages[i];
					GL.BeginVertical("OL box");
					GL.BeginHorizontal("box");
					GL.Label(value["name"].ToString());
					GL.Label(value["version"].ToString());

					if(Directory.Exists(value["localpath"].ToString()))
					{
						if(GL.Button("pack",GUILayout.Width(100)))
						{
							Export(value["name"].ToString(),value["localpath"].ToString());
						}
					}
					else if(ExistsLocalPackage(value["name"].ToString()) )
					{
						if(GL.Button("Import",GUILayout.Width(100)))
						{
							
						}
					}
					GL.EndHorizontal();
					GL.EndVertical();
				}
			}
		}




		private bool ExistsLocalPackage(string PackageName)
		{
			//TODO
			return true;
		}

        private void AddPackage(string name,string version,string loaclpath)
        {
			var index = GetPackageIndex(name);
			var package = new JsonData(); 
			package["name"] = name;
			package["version"] = version;
			package["localpath"] = loaclpath;
			BaseData["Packages"] = new JsonData(); 

			if(index == -1)
				BaseData["Packages"].Add(package);
			else
				BaseData["Packages"][index] = package;

			BaseData.SaveJsonData(PackageFilePath);

			AssetDatabase.Refresh();
        }

		private void Export(string name,string path)
		{
			var fileName = PackagesLocalPath + name + ".unitypackage";
			var list = PathUtil.GetFullPathName(path,true,SearchOption.AllDirectories,true);
			AssetDatabase.ExportPackage(list.ToArray(),fileName,ExportPackageOptions.Recurse);
			AssetDatabase.Refresh();
			ShowNotification(new GUIContent("打包完毕"));
		}

		private int GetPackageIndex(string name)
		{
			var packages = BaseData["Packages"];
			for(int i = 0;i<packages.Count;i++)
			{
				if(packages[i]["name"].ToString().Equals(name))
					return i;
			}
			return -1;
		}
    }
}
