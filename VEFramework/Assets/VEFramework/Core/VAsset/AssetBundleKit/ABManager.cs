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
    using System.Collections;

    ///<summary>
    ///规则：基于AssetPath(相对路径)来加载所需资源
    ///1.搜寻优先级：PersistentABDir/AppSetting.AppABResVersion/AssetPath 优先于 AssetBundleDir/AssetPath
    ///</summary>
    public class ABManager : VAssetManager<ABManager>
	{
		public override string ManagerName
        {
            get
            {
                return "ABManager";
            }
        }

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
		private Dictionary<string,ABAssurer> mABAssurerList;
        private List<Assurer> mWait4RecycleList;
        private List<Assurer> mRecycleList;
        private AssetBundleManifest mManifest;
        private ABAssurer ScriptLoader; 
		public override void Init()
		{
            base.Init();
			mRealABFilePath = new Dictionary<string, string>();
            mFilePathExistsList = new Dictionary<string, bool>();
			mABAssurerList = new Dictionary<string, ABAssurer>();
            mWait4RecycleList = new List<Assurer>();
            mRecycleList = new List<Assurer>();

            StartCoroutine(RecycleUselessAssurer());
            LoadManifest();
            InitScriptLoader();
		}

        private void LoadManifest()
        {
            var ManifestPath = AssetCustomSetting.ABManifestFileName;
            if(ManifestPath.IsEmptyOrNull())
                ManifestPath = "StreamingAssets";
            var assurer = GetAssurer(ManifestPath,false,false);
            assurer.FileName = "AssetBundleManifest";            
            mManifest = assurer.LoadSync<AssetBundleManifest>();
        }

        

        private void InitScriptLoader()
        {
            var ScriptPath = AssetCustomSetting.ScriptABPath;
            if(ScriptPath.IsEmptyOrNull())
                return;
            ScriptLoader = GetAssurerSync(ScriptPath);
            if(ScriptLoader != null)
                ScriptLoader.LogSwitch = false;
        }


    #region 对外方法
        public ABAssurer GetAssurerSync(string AssetPath)
        {
            var assurer = LoadSync(AssetPath);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }

        public ABAssurer GetAssurerAsync(string AssetPath)
        {
            var assurer = LoadAsync(AssetPath,null,false);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }

        public ABAssurer GetAssurerAsync<T>(string AssetPath,Action<T> finishCallback = null) where T:UnityEngine.Object
        {
            var assurer = LoadAsync(AssetPath,(ar)=>{GetResOnFinish<T>(ar as ABAssurer,finishCallback);},false);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }

        ///<param name="AssetPath">资产加载外部路径</param>
        ///<summary>
        /// 同步资产加载
        ///</summary>
        public override T LoadSync<T>(string AssetPath)
        {
            return LoadSync<T>(AssetPath,false);
        }

        ///<param name="AssetPath">资产加载外部路径</param>
        ///<param name="FinishCallback">资产加载回调</param>
        ///<summary>
        /// 异步资产加载
        ///</summary>
        public override void LoadAsync<T>(string AssetPath,Action<T> FinishCallback = null)
        {
            LoadAsync(AssetPath,(assurer)=>{GetResOnFinish<T>(assurer as ABAssurer,FinishCallback);},false);
        }

        public T LoadScript<T>(string FileName) where T:UnityEngine.Object
        {
            if(ScriptLoader == null)
                return null;
            Log.IColor("LoadScript:{0}",LogColor.Green,FileName);
            return ScriptLoader.Get<T>(FileName);
        }

    #endregion



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
            //TODO:更精确的锁定文件
            // var preciseFile = assetPath.Replace(abRealPath,"");
            // if(preciseFile.StartsWith("/"))
            // {
            //     preciseFile = preciseFile.Substring(1);
            //     fileName = preciseFile;
            // }
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
                abRealPath = AssetCustomSetting.PersistentABDir + assetPath;
            else
                abRealPath = AssetCustomSetting.PersistentABDir + AppSetting.AppABResVersion + "/" + assetPath;
            if (mFilePathExistsList.ContainsKey(abRealPath))
                return mFilePathExistsList[abRealPath];
            if (PathUtil.IsPersistentFileExists(abRealPath))
            {
                mFilePathExistsList.Add(abRealPath, true);
                return true;
            }
            abRealPath = AssetCustomSetting.AssetBundleDir + assetPath;
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
            abName = abName.Replace(AssetCustomSetting.AssetBundleDir,"");

            if(AppSetting.AppABResVersion.IsEmptyOrNull())
                abName = abName.Replace(AssetCustomSetting.PersistentABDir, "");
            else
                abName = abName.Replace(AssetCustomSetting.PersistentABDir + AppSetting.AppABResVersion + "/", "");
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


    #region 核心
        ///<param name="AssetPath">资产加载外部路径</param>
        ///<param name="bPostfix">资产文件是否存在后缀</param>
        ///<param name="bUnloadTag">资产释放模式</param>
        ///<summary>
        /// ABAssurer 唯一获取方式
        ///</summary>
        private ABAssurer GetAssurer(string AssetPath,bool bPostfix,bool bUnloadTag)
        {
            string realPath;
            bool isContains = false;
            if(mRealABFilePath.ContainsKey(AssetPath))
            {
                isContains = true;
                realPath = mRealABFilePath[AssetPath];
            }
            ABAssurer assurer = null;

            var ABAnlysis = ABPathAnalysis.EasyGet();
            ABAnlysis.Analyze(AssetPath,bPostfix,bUnloadTag);
            if(mABAssurerList.ContainsKey(ABAnlysis.RealPath))
            {
                assurer = mABAssurerList[ABAnlysis.RealPath];
                assurer.Init(ABAnlysis);
            }
            if(assurer == null)
            {
                assurer = ABAssurer.EasyGet();
                MarkAssurer(assurer);
                assurer.Init(ABAnlysis);
                mABAssurerList.Add(assurer.RealPath,assurer);
            }
            if(!isContains)
                mRealABFilePath.Add(assurer.AssetPath,assurer.RealPath);
            assurer.Retain();
            Log.IColor("ABAssurer[AssetPath:{0},RealPath:{1},FileName:{2}]",LogColor.Green,assurer.AssetPath,assurer.RealPath,assurer.FileName);
            return assurer;
        }

    #endregion

    #region Sync Load
        protected T LoadSync<T>(string AssetPath,bool bDepLoad) where T : UnityEngine.Object
        {
            var assurer = LoadSync(AssetPath,bDepLoad);
            return assurer.Get<T>();
        }

        protected ABAssurer LoadSync(string AssetPath)
        {
            return LoadSync(AssetPath,false);
        }
        protected ABAssurer LoadSync(string AssetPath,bool bDepLoad)
        {
            var assurer = GetAssurer(AssetPath,true,DefaultUnLoadTag);
            if(bDepLoad == false)
                LoadDependenciesSync(GetAssetBundleName(assurer.RealPath),ref assurer.DependFileList);
            else
                assurer.AutoRelease = false;
            assurer.LoadSync();
            return assurer;
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
            {
                callback(null);
                return;
            }
            callback(assurer.Get<T>());
        }
        protected void LoadAsync(string AssetPath,Action<Assurer> finishCallback = null)
        {
            LoadAsync(AssetPath,finishCallback,false);
        }
        protected ABAssurer LoadAsync(string AssetPath,Action<Assurer> finishCallback,bool bDepLoad)
        {
            var assurer = GetAssurer(AssetPath,true,DefaultUnLoadTag);
            if(bDepLoad == false)
                LoadDependenciesAsync(GetAssetBundleName(assurer.RealPath),ref assurer.DependFileList,ref assurer.DependAssurerList);
            else
                assurer.AutoRelease = false;
            if(finishCallback != null)
                assurer.LoadFinishCallback += finishCallback;
            assurer.LoadAsync();
            return assurer;
        }

        protected void LoadDependenciesAsync(string abName,ref string[] dflist,ref List<ABAssurer> dAssurerlist)
        {
            if (mManifest == null)
                return;
            abName = GetAssetBundleRelativeName(abName);
            dflist = mManifest.GetAllDependencies(abName);
            dAssurerlist = new List<ABAssurer>();
            for(int i = 0;i < dflist.Length;i++)
            {
                if(string.IsNullOrEmpty(dflist[i]))
                    continue;
                dAssurerlist.Add(LoadAsync(dflist[i],null,true));
            }
        }
    #endregion


    #region  管理
        public override void OnAssurerLoaded(Assurer assurer)
		{
            var aber = assurer as ABAssurer;
            if(aber == null)
                return;
            if(aber.AutoRelease)
                aber.Release();
		}
        public override void OnAssurerLoadedFail(Assurer assurer)
		{
            var aber = assurer as ABAssurer;
            if(aber == null)
                return;
            if(aber.AutoRelease)
                aber.ForceRecycle();
		}


        public override void RecycleAssurer(Assurer aber)
        {
            WaitForRecycle(aber);
        }
        public override void ReUseAssurer(Assurer aber)
        {
            mWait4RecycleList.Remove(aber);
        }

        public void WaitForRecycle(Assurer aber)
        {
            mWait4RecycleList.Add(aber);
        }

        IEnumerator RecycleUselessAssurer()
        {
            while(true)
            {
                mRecycleList.Clear();
                mWait4RecycleList.ForEach((aber)=>{
                    if(aber.UseCount <= 0)
                    {
                        if(aber.KeepTime <= 0)
                        {
                            mRecycleList.Add(aber);
                        }
                        else
                        {
                            if(AssetCustomSetting.AssetUnLoadMode == AssetUnLoadModeType.I_DONT_CARE)
                            {
                                aber.KeepTime -= Time.deltaTime; 
                            }
                            else
                            {
                                mRecycleList.Add(aber);
                            }
                        }     
                    }
                });
                yield return null;
                if(mRecycleList.Count > 0)
                {
                    mRecycleList.ForEach((aber)=>{mWait4RecycleList.Remove(aber);aber.RecycleSelf();});
                    mRecycleList.Clear();
                }
            }
        }

        public void UnloadAsset(AssetBundle ab,bool tag)
        {
            ab.Unload(tag);
        }

        public bool RemoveAssurer(ABAssurer aber)
		{
            if(aber == null || aber.RealPath.IsEmptyOrNull())
                return false;
			if(mABAssurerList.ContainsKey(aber.RealPath))
			{
				mABAssurerList.Remove(aber.RealPath);
				return true;
			}
			return true;	
		}

        public void ReleaseDepend(string[] depList)
        {
            if(depList == null || depList.Length ==0)
                return;
            depList.ForEach(key =>{
                if(mRealABFilePath.ContainsKey(key))
                {
                    if(mABAssurerList.ContainsKey(mRealABFilePath[key]))
                    {
                        mABAssurerList[mRealABFilePath[key]].Release();
                    }
                }
            });
        }

        public void ReleaseDepend(string[] depList,bool releaseMode)
        {
            if(depList == null || depList.Length ==0)
                return;
            depList.ForEach(key =>{
                if(mRealABFilePath.ContainsKey(key))
                {
                    if(mABAssurerList.ContainsKey(mRealABFilePath[key]))
                    {
                        mABAssurerList[mRealABFilePath[key]].Release(releaseMode);
                    }
                }
            });
        }
    #endregion
    
    #region Editor
        public override Dictionary<string,Assurer> GetAssurerList()
        {
            mAssurerList.Clear();
            mABAssurerList.ForEach(assurer=>{
                mAssurerList.Add(assurer.Key,assurer.Value);
            });
            return mAssurerList;
        }
    #endregion


    }
}