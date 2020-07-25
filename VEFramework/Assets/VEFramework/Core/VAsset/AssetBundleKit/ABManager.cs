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
	using System.Collections.Generic;
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

		private Dictionary<string,string> mRealABFilePath;
        private Dictionary<string, bool> mFilePathExistsList;
        ///<summary>
        /// Key:RealPath  
        ///</summary>
		private Dictionary<string,ABAssurer> mAssurerList;

		public override void Init()
		{
			mAsyncTaskStack = new LinkedList<IAsyncTask>();
			mRealABFilePath = new Dictionary<string, string>();
            mFilePathExistsList = new Dictionary<string, bool>();
			mAssurerList = new Dictionary<string, ABAssurer>();
		}

        # region Check Function
        public string GetAssetbundleRealPath(string assetPath,ref string fileName)
        {
            string filePath = "";
            PathUtil.GetPathAndFile(assetPath,ref filePath,ref fileName);
            int iIdx = assetPath.LastIndexOf(AssetCustomSetting.ABPostfix);
            if(-1 == iIdx)
                assetPath += AssetCustomSetting.ABPostfix;
            string abRealPath = string.Empty ;
            if (false == CheckFileExists(assetPath, ref abRealPath))
            {
                Log.W("{0} nonexistent",assetPath);
                if(false == CheckFileExists(filePath, ref abRealPath))
                    Log.W("{0} nonexistent",filePath);
            }
            return abRealPath;
        }


        public bool CheckFileExists(string assetPath,ref string abRealPath, bool bTolower = true)
        {
            if (bTolower)
                assetPath = assetPath.ToLower();

            if (mRealABFilePath.ContainsKey(assetPath)) {
                abRealPath = mRealABFilePath[assetPath];
                if (mFilePathExistsList.ContainsKey(abRealPath))
                {
                    return mFilePathExistsList[abRealPath];
                }
                return false;
            }
            if(AppSetting.AppABResVersion.IsEmptyOrNull())
                abRealPath = AssetCustomSetting.PersistentDir + "/" + assetPath;
            else
                abRealPath = AssetCustomSetting.PersistentDir + "/" + AppSetting.AppABResVersion + "/" + assetPath;
            if (PathUtil.IsPersistentFileExists(abRealPath))
            {
                mRealABFilePath.Add(assetPath, abRealPath);
                mFilePathExistsList.Add(abRealPath, true);
                return true;
            }

            abRealPath = AssetCustomSetting.AssetBundleDir + "/" + assetPath;
            if (PathUtil.IsStreamingFileExists(abRealPath))
            {
                mRealABFilePath.Add(assetPath, abRealPath);
                mFilePathExistsList.Add(abRealPath, true);
                return true;
            }
            mRealABFilePath.Add(assetPath, abRealPath);
            mFilePathExistsList.Add(abRealPath, false);
            return false;
        }



        # endregion


        public T LoadSync<T>(string AssetPath) where T : UnityEngine.Object
        {
            string realPath = AssetPath;
            bool isContains = false;
            if(mRealABFilePath.ContainsKey(AssetPath))
            {
                isContains = true;
                realPath = mRealABFilePath[AssetPath];
            }
            ABAssurer assurer = null;
            if(mAssurerList.ContainsKey(realPath))
                assurer = mAssurerList[realPath];
            if(assurer == null)
            {
                assurer = ABAssurer.EasyGet(AssetPath);
                mAssurerList.Add(assurer.RealPath,assurer);
            }
            if(!isContains)
                mRealABFilePath.Add(assurer.AssetPath,assurer.RealPath);
            return assurer.LoadSync<T>();
        }


		public bool RemoveAssurer(ABAssurer aber)
		{
			if(mAssurerList.ContainsKey(aber.AssetPath))
			{
				mAssurerList.Remove(aber.AssetPath);
				return true;
			}
			return true;	
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
	}
}