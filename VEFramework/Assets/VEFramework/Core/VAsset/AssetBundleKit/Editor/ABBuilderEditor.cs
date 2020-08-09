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
    using UnityEditor;
    using UnityEngine;

    public class ABBuilderEditor:EditorWindow
	{
		private void OnGUI() {}
		[MenuItem ("VETool/VAsset/Create/CreateAssetBundleRule",false,0)]
		public static void CreateAssetBundleRule()
		{
			Log.IColor("CreateAssetBundleRule",LogColor.OrangeRed);
			if(File.Exists(AssetCustomSetting.AssetBundlerRuleAssetPath))
				Log.E("AssetBundleReulAsset already existed");
			else
			{
				ABRuleAsset ruleAsset = ScriptableObject.CreateInstance<ABRuleAsset>();
				var AssetPath = AssetCustomSetting.AssetBundlerRuleAssetPath.Replace(Application.dataPath,"Assets");
				AssetDatabase.CreateAsset(ruleAsset,AssetPath);
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		[MenuItem ("VETool/VAsset/Open/OpenPersistentDataPath",false,1)]
		public static void OpenPersistentDataPath()
		{
			string path = AssetCustomSetting.PersistentABDir.Replace('/', '\\');
			System.Diagnostics.Process.Start("explorer.exe", path);
		}


		[MenuItem ("VETool/VAsset/Open/OpenExternalAssetPath",false,1)]
		public static void OpenExternalAssetPath()
		{
			string path = AssetCustomSetting.ExternalAssetDir.Replace('/', '\\');
			System.Diagnostics.Process.Start("explorer.exe", path);
		}


		[MenuItem ("VETool/VAsset/Clear/ClearExternalAsset",false,2)]
		public static void ClearExternalAsset()
		{
			string path = AssetCustomSetting.ExternalAssetDir.Replace('/', '\\');
			if(Directory.Exists(path))
			{
				Directory.Delete(path,true);
				PathUtil.CreateDirectory(path);
				Log.IColor("ClearExternalAsset",LogColor.OrangeRed);
			}
		}


		[MenuItem ("VETool/VAsset/Clear/ClearAllABName",false,2)]
		public static void ClearAllABName()
		{
			Log.IColor("ClearAllABName",LogColor.OrangeRed);
        	string[] names = AssetDatabase.GetAllAssetBundleNames();
			for (int j = 0; j < names.Length; j++)
				AssetDatabase.RemoveAssetBundleName(names[j], true);
			if (names.Length > 0)
				AssetDatabase.Refresh();
			else
				Log.E("Not Find ABName");
		}

		[MenuItem ("VETool/VAsset/Clear/DeleteAssetBundleDirectory",false,2)]
		public static void DeleteAssetBundleDirectory()
		{
			Log.IColor("Delete AssetBundle Directory",LogColor.OrangeRed);
			var ABDir = AssetCustomSetting.AssetBundleDir.EndWithAndRemove("/");

			if (File.Exists(ABDir))
			{
				FileUtil.DeleteFileOrDirectory(ABDir);
				AssetDatabase.Refresh();
			}
			else
			{
				Log.E("Not Find AssetBundle Directory:{0}",ABDir);
			}
		}
		
		[MenuItem ("VETool/VAsset/AssetBundleEasyBuild")]
		public static void EasyBuild()
		{
			AssetBundleEasyBuild();
			Log.IColor("Build Over",LogColor.OrangeRed);
		}

		[MenuItem ("VETool/VAsset/SetABNameByRule")]
		public static void SetABNameByRule()
		{
			SetABName4Rule();
		}
		[MenuItem ("VETool/VAsset/OnlyBuildAB")]
		public static void Build()
		{
			OnlyBuildAB();
			Log.IColor("Build Over",LogColor.OrangeRed);
		}


		[MenuItem ("Assets/VAsset/SetABNameBySelect")]
		public static void SetABNameBySelect()
		{
			GetABBuilder().SetAssetBundleNameBySelectList(GetSelectedPathList());;
			AssetDatabase.Refresh();
		}


	#region General function
		public static ABBuilder GetABBuilder()
		{
			var Builder = new ABBuilder(ABBuilder.AssetBundleBuildTarget);
			return Builder;
		}
		public static ABBuilder GetABBuilder(BuildTarget Target)
		{
			var Builder = new ABBuilder(Target);
			return Builder;
		}
		public static ABRuleAsset GetABRuleAsset()
		{
			var ABRulePath = AssetCustomSetting.AssetBundlerRuleAssetPath;
			ABRulePath = ABRulePath.Replace(UnityEngine.Application.dataPath,"Assets");
			ABRuleAsset Rule = AssetDatabase.LoadAssetAtPath<ABRuleAsset>(ABRulePath);
			if (Rule == null)
			{
				Log.E("ABRuleAsset inexistence:{0}",ABRulePath);
				return null;
			}
			return Rule;
		}

		public static bool SetABName4Rule()
		{
			Log.IColor("SetABNameByRule",LogColor.OrangeRed);
			var Rule = GetABRuleAsset();
			if(Rule == null)
				return false;
			var Builder = GetABBuilder();
			Builder.SetAssetBundleNameByRule(Rule);
			AssetDatabase.Refresh();
			return true;
		}

		public static bool OnlyBuildAB()
		{
			var Builder = GetABBuilder();
			Builder.Build();
			return true;
		}

		public static bool AssetBundleEasyBuild()
		{
			var Builder = GetABBuilder();
			var Rule = GetABRuleAsset();
			if(Rule != null)
			{
				Builder.SetAssetBundleNameByRule(Rule);
			}
			Builder.Build();
			return true;
		}

		public static List<string> GetSelectedPathList()
		{
			var pathList = new List<string>();
			foreach (var obj in Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.Assets))
			{
				var path = AssetDatabase.GetAssetPath(obj);
				if (!string.IsNullOrEmpty(path))
				{
					pathList.Add(path);
				}
			}
			return pathList;
		}
	#endregion
	}
}
