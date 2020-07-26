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
namespace VEFramework
{
	using UnityEngine;
	using System.Collections.Generic;
    using System;

    public class ABManager : VEManagers<ABManager>
	{
		public override string ManagerName
        {
            get
            {
                return "ABManager";
            }
        }
		private int	mCurrentCoroutineCount = 0;
        private int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        private	LinkedList<IAsyncTask> mAsyncTaskStack;
        ///<summary>
        /// Key:AssetPath 
        ///</summary>
		private Dictionary<string,string> mRealABFilePath;
        ///<summary>
        /// Key:RealPath 
        ///</summary>
        private Dictionary<string, bool> mFilePathExistsList;
        ///<summary>
        /// Key:RealPath  
        ///</summary>
		private Dictionary<string,ABAssurer> mAssurerList;
        private AssetBundleManifest mManifest;
		public override void Init()
		{
			mAsyncTaskStack = new LinkedList<IAsyncTask>();
			mRealABFilePath = new Dictionary<string, string>();
            mFilePathExistsList = new Dictionary<string, bool>();
			mAssurerList = new Dictionary<string, ABAssurer>();
            LoadManifest();
		}

        private void LoadManifest()
        {
            var ManifestPath = AssetCustomSetting.ABManifestFileName;
            if(ManifestPath.IsEmptyOrNull())
                ManifestPath = "StreamingAssets";
            var assurer = GetAssurer(ManifestPath,false);
            assurer.FileName = "AssetBundleManifest";            
            mManifest = assurer.LoadSync<AssetBundleManifest>();
            assurer.Release();
        }

    # region Check Function
        public string GetAssetbundleRealPath(string assetPath,ref bool fileExist,ref string fileName,bool bPostfix = true)
        {
            string filePath = "";
            PathUtil.GetPathAndFile(assetPath,ref filePath,ref fileName);
            int iIdx = assetPath.LastIndexOf(AssetCustomSetting.ABPostfix);
            if(-1 == iIdx && bPostfix)
                assetPath += AssetCustomSetting.ABPostfix;
            string abRealPath = string.Empty;
            fileExist = true;
            if (false == CheckFileExists(assetPath, ref abRealPath))
            {
                Log.W("{0} nonexistent",abRealPath);
                int kIdx = filePath.LastIndexOf(AssetCustomSetting.ABPostfix);
                if(-1 == kIdx && bPostfix)
                    filePath += AssetCustomSetting.ABPostfix;
                if(false == CheckFileExists(filePath, ref abRealPath))
                {
                    Log.W("{0} nonexistent",abRealPath);
                    fileExist = false;
                }
            }
            return abRealPath;
        }

        protected bool CheckFileExists(string assetPath,ref string abRealPath, bool bTolower = true)
        {
            if (mRealABFilePath.ContainsKey(assetPath)) {
                abRealPath = mRealABFilePath[assetPath];
                if (mFilePathExistsList.ContainsKey(abRealPath))
                {
                    return mFilePathExistsList[abRealPath];
                }
                return false;
            }

            if (bTolower)
                assetPath = assetPath.ToLower();

            if(AppSetting.AppABResVersion.IsEmptyOrNull())
                abRealPath = AssetCustomSetting.PersistentABDir + "/" + assetPath;
            else
                abRealPath = AssetCustomSetting.PersistentABDir + "/" + AppSetting.AppABResVersion + "/" + assetPath;
            if (mFilePathExistsList.ContainsKey(abRealPath))
                return mFilePathExistsList[abRealPath];
            if (PathUtil.IsPersistentFileExists(abRealPath))
            {
                mFilePathExistsList.Add(abRealPath, true);
                return true;
            }
            abRealPath = AssetCustomSetting.AssetBundleDir + "/" + assetPath;
            if (mFilePathExistsList.ContainsKey(abRealPath))
                return mFilePathExistsList[abRealPath];
            if (PathUtil.IsStreamingFileExists(abRealPath))
            {
                mFilePathExistsList.Add(abRealPath, true);
                return true;
            }
            mFilePathExistsList.Add(abRealPath, false);
            return false;
        }

        protected string GetAssetBundleName(string abRealPath)
        {
            var abName = abRealPath;
            abName = abName.Replace(AssetCustomSetting.AssetBundleDir +"/","");

            if(AppSetting.AppABResVersion.IsEmptyOrNull())
                abName = abName.Replace(AssetCustomSetting.PersistentABDir + "/", "");
            else
                abName = abName.Replace(AssetCustomSetting.PersistentABDir + "/" + AppSetting.AppABResVersion + "/", "");
            return abName;
        }

        protected string GetAssetBundleRelativeName(string abName)
        {
            string relativeName = string.Empty;
            relativeName = abName.Replace(Application.dataPath + "/","");
            relativeName = relativeName.ToLower();

            int iIdx = relativeName.LastIndexOf(AssetCustomSetting.ABPostfix);
            if(-1 == iIdx)
                relativeName += AssetCustomSetting.ABPostfix;
            return relativeName;
        }
    # endregion


    #region 已扩展方式
        public ABAssurer GetAssurer(string AssetPath,bool bPostfix = true)
        {
            string realPath;
            bool isContains = false;
            if(mRealABFilePath.ContainsKey(AssetPath))
            {
                isContains = true;
                realPath = mRealABFilePath[AssetPath];
            }
            ABAssurer assurer = null;
            //TODO 优化RealPath过程
            bool bFileExist = false;
            string fileName = string.Empty;
            realPath = GetAssetbundleRealPath(AssetPath,ref bFileExist,ref fileName);
            if(mAssurerList.ContainsKey(realPath))
                assurer = mAssurerList[realPath];
            if(assurer == null)
            {
                assurer = ABAssurer.EasyGet(AssetPath,bPostfix);
                mAssurerList.Add(assurer.RealPath,assurer);
            }
            if(!isContains)
                mRealABFilePath.Add(assurer.AssetPath,assurer.RealPath);
            assurer.Retain();
            return assurer;
        }
    #endregion

    #region Sync Load
        public ABAssurer LoadSync(string AssetPath)
        {
            return LoadSync(AssetPath,false);
        }
        public ABAssurer LoadSync(string AssetPath,bool bDepLoad)
        {
            var assurer = GetAssurer(AssetPath);
            if(bDepLoad == false)
                LoadDependenciesSync(GetAssetBundleName(assurer.RealPath),ref assurer.DependFileList);
            assurer.LoadSync();
            return assurer;
        }
        public T LoadSync<T>(string AssetPath) where T : UnityEngine.Object
        {
            return LoadSync<T>(AssetPath,false);
        }
        public T LoadSync<T>(string AssetPath,bool bDepLoad) where T : UnityEngine.Object
        {
            var assurer = LoadSync(AssetPath,bDepLoad);
            return assurer.Get<T>();
        }

        protected void LoadDependenciesSync(string abName,ref string[] dflist)
        {
            if (mManifest == null)
                return;
            abName = GetAssetBundleRelativeName(abName);
            dflist = mManifest.GetAllDependencies(abName);
            for(int i = 0;i < dflist.Length;i++)
            {
                if(string.IsNullOrEmpty(dflist[i]))
                    continue;
                LoadSync(dflist[i],true);
            }
        }
    #endregion
    #region Async Load
        private void GetResOnFinish<T>(ABAssurer assurer,Action<T> callback) where T:UnityEngine.Object
        {
            if(callback == null)
                return;
            if(assurer == null)
                callback(null);
            callback(assurer.Get<T>());
        }
        public void LoadAsync<T>(string AssetPath,Action<T> finishCallback = null) where T:UnityEngine.Object
        {
            LoadAsync(AssetPath,(assurer)=>{GetResOnFinish<T>(assurer,finishCallback);},false);
        }

        public void LoadAsync(string AssetPath,Action<ABAssurer> finishCallback = null)
        {
            LoadAsync(AssetPath,finishCallback,false);
        }
        public void LoadAsync(string AssetPath,Action<ABAssurer> finishCallback,bool bDepLoad)
        {
            var assurer = GetAssurer(AssetPath);
            if(bDepLoad == false)
                LoadDependenciesAsync(GetAssetBundleName(assurer.RealPath),ref assurer.DependFileList);
            if(finishCallback != null)
                assurer.LoadFinishCallback += finishCallback;
            assurer.LoadAsync();
        }

        protected void LoadDependenciesAsync(string abName,ref string[] dflist)
        {
            if (mManifest == null)
                return;
            abName = GetAssetBundleRelativeName(abName);
            dflist = mManifest.GetAllDependencies(abName);
            for(int i = 0;i < dflist.Length;i++)
            {
                if(string.IsNullOrEmpty(dflist[i]))
                    continue;
                LoadAsync(dflist[i],null,true);
            }
        }
    #endregion


        public void PushInAsyncList(IAsyncTask task)
        {
            if (task == null)
            {
                return;
            }
            mAsyncTaskStack.AddLast(task);
            TryStartNextAsyncTask();
        }

		private void OnAsyncTaskFinish()
        {
            --mCurrentCoroutineCount;
            TryStartNextAsyncTask();
        }

        private void TryStartNextAsyncTask()
        {
            if (mAsyncTaskStack.Count == 0)
            {
                return;
            }

            if (mCurrentCoroutineCount >= mMaxCoroutineCount)
            {
                return;
            }
            var task = mAsyncTaskStack.First.Value;
            mAsyncTaskStack.RemoveFirst();
            ++mCurrentCoroutineCount;
            StartCoroutine(task.DoLoadAsync(OnAsyncTaskFinish));
        }

        public bool RemoveAssurer(ABAssurer aber)
		{
            if(aber == null || aber.RealPath.IsEmptyOrNull())
                return false;
			if(mAssurerList.ContainsKey(aber.RealPath))
			{
				mAssurerList.Remove(aber.RealPath);
				return true;
			}
			return true;	
		}
	}
}