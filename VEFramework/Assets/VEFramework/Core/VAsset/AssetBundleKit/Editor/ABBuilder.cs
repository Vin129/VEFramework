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
	using UnityEngine;
	using UnityEditor;
	using System.IO;
	using System.Collections.Generic;
	public class ABBuilder
	{
		public ABBuilder(BuildTarget kTarget)
		{
			mBuildTarget = kTarget;
		}

		public void BuildFromRuleAsset(ABRuleAsset RuleAsset, string outputPath = null)
		{
			SetAssetBundleNameByRule(RuleAsset);
			Build(outputPath);
		}

		public void Build(string outputPath = null)
		{ 
			string buildFolder;
			if (string.IsNullOrEmpty (outputPath)) {
				buildFolder = AssetCustomSetting.ABFileBuildPath;
			} else {
				buildFolder = outputPath;
			}			
			if(!PathUtil.IsFileExists(buildFolder))
				PathUtil.CreateDirectory(buildFolder);

			BuildPipeline.BuildAssetBundles(buildFolder,m_BuildOption,mBuildTarget);
			AssetDatabase.Refresh();
			// EncryptProcess();
		}

		// public void Build(string strIn,SearchOption kSearchOpt = SearchOption.TopDirectoryOnly )
		// {
		// 	if(false == PathUtil.IsDirectory(strIn))
		// 		return;
		// 	string strABName = GetABName(strIn);
		// 	if(null == strABName)
		// 		return ;
		// 	string strAssetPath = PathUtil.GetPathUnderAssets(strIn);
		// 	List<string> strFileList = PathUtil.GetFileNameWithPostfix(strIn,strAssetPath,kSearchOpt);
		// 	if(0 == strFileList.Count)
		// 		return;
		// 	string strOutPath = PathUtil.Res2ABPathConvert(strAssetPath);
		// 	int iIdx = strOutPath.LastIndexOf("/"+strABName);
		// 	if( -1 != iIdx)
		// 		strOutPath = strOutPath.Substring(0,iIdx);
		// 	PathUtil.CreateDirectory(Application.dataPath+"/../" + strOutPath);
		// 	AssetBundleBuild[] kBuildMap = new AssetBundleBuild[1];
		// 	kBuildMap[0].assetBundleName = strABName + AssetCustomSetting.ABPostfix;
		// 	kBuildMap[0].assetNames = strFileList.ToArray();
		// 	Log.I(strOutPath);
		// 	AssetBundleManifest kManifest = BuildPipeline.BuildAssetBundles(strOutPath,kBuildMap,m_BuildOption,mBuildTarget);
		// 	if(null == kManifest)
		// 		Debug.LogError("Build AssetBundle Failed");
		// }


		public void SetAssetBundleNameByRule(ABRuleAsset RuleAsset)
		{
			if (null == RuleAsset)
				return;
			List<ABRuleAsset.ABRuleData> mRuleDatas = RuleAsset.GetABRuleDatas();
			for (int i = 0; i < mRuleDatas.Count; i++)
			{
				var ruleData = mRuleDatas[i];
				string AssestPath = Application.dataPath + "/" + ruleData.Path;
				switch (ruleData.BuildType)
				{
					case ABRuleAsset.ABBuildType.AllPack:
						SetAllInOnePack(AssestPath);
						break;
					case ABRuleAsset.ABBuildType.PathPack:
						SetPerPathOnePack(AssestPath);
						break;
					case ABRuleAsset.ABBuildType.FilePack:
						SetPerFileOnePack(AssestPath);
						break;
				}
			}
		}

		public void SetAssetBundleNameBySelectList(List<string> selectList)
		{
			if(selectList.Count == 0)
				return;
			selectList.ForEach
			(
				(path)=>
				{
					var AssestPath = path.Replace("Assets/",Application.dataPath + "/");
					switch (JudgeAssetPath(AssestPath))
					{
						// case ABRuleAsset.ABBuildType.AllPack:
						// 	SetAllInOnePack(AssestPath);
						// 	break;
						case ABRuleAsset.ABBuildType.PathPack:
							SetPerPathOnePack(AssestPath);
							break;
						case ABRuleAsset.ABBuildType.FilePack:
							SetPerFileOnePack(AssestPath);
							break;
					}
				}
			);
		}

		public void CopyABNameBySelectList(List<string> selectList)
		{
			if(selectList.Count == 0)
				return;
			string copyContent = string.Empty;
			selectList.ForEach
			(
				(path)=>
				{
					var AssestPath = path.Replace("Assets/",Application.dataPath + "/");
					switch (JudgeAssetPath(AssestPath))
					{
						// case ABRuleAsset.ABBuildType.AllPack:
						// 	SetAllInOnePack(AssestPath);
						// 	break;
						case ABRuleAsset.ABBuildType.PathPack:
							GetPerPathABName(AssestPath,ref copyContent);
							break;
						case ABRuleAsset.ABBuildType.FilePack:
							GetPerFileABName(AssestPath,ref copyContent);
							break;
					}
				}
			);
			if(!copyContent.Equals(string.Empty))
			{
				UnityEngine.GUIUtility.systemCopyBuffer = copyContent;
				Log.IColor(copyContent,LogColor.OrangeRed);
			}
		}



		//整包
		private void SetAllInOnePack(string AssestPath)
		{
			if(false == PathUtil.IsDirectory(AssestPath))
				return;

			EditorUtility.DisplayProgressBar("Set AssetName", "Setting AssetName...", 0f);
			string strABName = GetABName(AssestPath);
			if(null == strABName)
				return ;
			List<string> strFileList = PathUtil.GetFullPathName(AssestPath,true,SearchOption.AllDirectories);
			for(int i = 0;i < strFileList.Count;i++)
			{
				string strFullName = strFileList[i];
				strFullName = strFullName.Replace(Application.dataPath,"Assets");
				strFileList[i] = strFullName;
			}
			SetAssetsBundleName(strFileList,strABName,false);
			EditorUtility.ClearProgressBar(); 
		}

		//目录分包
		private void SetPerPathOnePack(string AssestPath)
		{
			List<string> strDirs = PathUtil.GetAllPath(AssestPath);
			foreach(var dir in strDirs)
			{
				if(false == PathUtil.IsDirectory(dir))
					return;
				EditorUtility.DisplayProgressBar("Set AssetName", "Setting AssetName...", 0f);
				string strABName = GetABName(dir);
				if(null == strABName)
					return ;
				string strAssetPath = PathUtil.GetPathUnderAssets(dir);
				List<string> strFileList = PathUtil.GetFileNameWithPostfix(dir,strAssetPath,SearchOption.TopDirectoryOnly);
				SetAssetsBundleName(strFileList,strABName,false);
				EditorUtility.ClearProgressBar(); 
			}
		}
		private void GetPerPathABName(string AssestPath,ref string content)
		{
			List<string> strDirs = PathUtil.GetAllPath(AssestPath);
			foreach(var dir in strDirs)
			{
				if(false == PathUtil.IsDirectory(dir))
					return;
				string strABName = GetABName(dir);
				if(null == strABName)
					return;
				if(content.Equals(string.Empty))
					content = strABName;
				else
					content += "," + strABName;
			}
		}



		//文件分包
		private void SetPerFileOnePack(string AssestPath)
		{
			List<string> strFiles = PathUtil.GetAllFiles(AssestPath);
			foreach(var file in strFiles)
			{
				if(false == PathUtil.IsFileExists(file))
					return;
				EditorUtility.DisplayProgressBar("Set AssetName", "Setting AssetName...", 0f);
				string strABName = GetABName(file);
				if(null == strABName)
					return ;
				string strAssetPath = PathUtil.GetPathUnderAssets(file,false);
				SetAssetBundleName(strAssetPath,strABName,false);
				EditorUtility.ClearProgressBar(); 
			}
		}

		private void GetPerFileABName(string AssestPath,ref string content)
		{
			List<string> strFiles = PathUtil.GetAllFiles(AssestPath);
			foreach(var file in strFiles)
			{
				if(false == PathUtil.IsFileExists(file))
					return;
				string strABName = GetABName(file);
				if(null == strABName)
					return ;
				if(content.Equals(string.Empty))
					content = strABName;
				else
					content += "," + strABName;
			}
		}


		private string GetABName(string strPath)
		{
			if(string.IsNullOrEmpty(strPath))
				return null;
			strPath = strPath.Replace("\\","/");
			if(strPath.Contains(AssetCustomSetting.ResourceDir))
				strPath = strPath.Replace(AssetCustomSetting.ResourceDir,"");
			else
				strPath = strPath.Replace(Application.dataPath + "/","");
			int iIdx = strPath.LastIndexOf('.');
			if( -1 != iIdx)
				strPath = strPath.Substring(0,iIdx);
			return strPath;
		}
		private void SetAssetsBundleName(List<string> strFileList,string strABName,bool bContainDependences)
		{
			for(int i = 0;i < strFileList.Count;i++)
			{
				string strFileName = strFileList[i];
				EditorUtility.DisplayProgressBar("Set AssetName", "Setting AssetName...", 1.0f*i/strFileList.Count);
				SetAssetBundleName(strFileName,strABName,bContainDependences);
			}
		}
		private void SetAssetBundleName(string strFileName,string strABName,bool bContainDependences)
		{
			AssetImporter kAssetImporter = AssetImporter.GetAtPath(strFileName);
			if (kAssetImporter && 0 != kAssetImporter.assetBundleName.CompareTo(strABName + AssetCustomSetting.ABPostfix))
			{
				kAssetImporter.assetBundleName = strABName + AssetCustomSetting.ABPostfix; 
			}
			if (bContainDependences)
			{ 
				string[] strDependences = AssetDatabase.GetDependencies(strFileName);
				for (int j = 0; j < strDependences.Length; j++)
				{
					if (strDependences[j].Contains(strABName) || strDependences[j].Contains(".cs"))
						continue;
					else
					{
						AssetImporter importer2 = AssetImporter.GetAtPath(strDependences[j]);
						string dpName = AssetDatabase.AssetPathToGUID(strDependences[j]); 
						importer2.assetBundleName = "alldependencies/" + dpName + AssetCustomSetting.ABPostfix;
					}
				}
			}
		}
		private ABRuleAsset.ABBuildType JudgeAssetPath(string AssetPath)
		{
			if(PathUtil.IsDirectory(AssetPath))
				return ABRuleAsset.ABBuildType.PathPack;
			else
				return ABRuleAsset.ABBuildType.FilePack;
		} 










		#region Encrypt Module
		// public void EncryptProcess()
		// {
		// 	Debug.Log("Begin Encrypt Process");
		// 	// 遍历StreamingAsset目录 
		// 	string strABRootDir = AssetCustomSetting.AssetBundleDir;
		// 	if (false == Directory.Exists(strABRootDir))
		// 		return;
		// 	for (int i = 0; i < AssetCustomSetting.EncryptList.Length; i++)
		// 	{
		// 		var valueChar = AssetCustomSetting.EncryptList[i];
		// 		string[] strFileNames = Directory.GetFiles(strABRootDir, valueChar + AssetCustomSetting.ABPostfix, SearchOption.AllDirectories);
		// 		foreach (var fileName in strFileNames)
		// 		{
		// 			if (false == File.Exists(fileName))
		// 				continue;
		// 			Debug.Log("加密："+ fileName);
		// 			FileAttributes kAttribute = File.GetAttributes(fileName);
		// 			kAttribute &= ~FileAttributes.ReadOnly;
		// 			FileStream kFileStream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite);
		// 			byte[] buff = new byte[kFileStream.Length];
		// 			kFileStream.Read(buff, 0, (int)kFileStream.Length);
		// 			buff = XXTEA.Encrypt(buff, AssetCustomSetting.EncryptKey);
		// 			kFileStream.Close();
		// 			File.Delete(fileName);

		// 			FileStream kWriteStream = new FileStream(fileName, FileMode.Create);
		// 			kWriteStream.Write(buff, 0, buff.Length);
		// 			buff = null;
		// 			kWriteStream.Close();
		// 		}
		// 	}
		// 	Debug.Log("End Encrypt Process");
		// }
		#endregion Encrypt Module

		private BuildTarget mBuildTarget;
		private BuildAssetBundleOptions m_BuildOption = BuildAssetBundleOptions.DeterministicAssetBundle;

		public static BuildTarget AssetBundleBuildTarget
		{
			get
			{
				#if UNITY_ANDROID
            		return BuildTarget.Android;
				#elif UNITY_IOS
            		return BuildTarget.iOS;
				#else
            		return BuildTarget.StandaloneWindows;
				#endif
			}
		}
	}
}