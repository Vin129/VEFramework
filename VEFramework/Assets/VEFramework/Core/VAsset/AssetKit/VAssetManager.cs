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
    using System;
	using System.Collections.Generic;

    public class VAssetManager<T> : VAssetManager where T : MonoManager
    {
        private static T mInstance;
        public static T Instance
        {
            get
            {
                if(mInstance == null)
                {
                    mInstance = VEManager.Instance.GetManagers<T>();
                    mInstance.Init();
                }
                return mInstance;
            }
        }
    }

    public abstract class VAssetManager : MonoManager,IAsyncTaskContainer,IAssurerContainer
    {
        public override string ManagerName
        {
            get
            {
                return "VAssetManager";
            }
        }

        ///<summary>
        ///不关心资源的释放问题
        ///TODO:是否要关心...
        ///</summary>
        public bool DefaultUnLoadTag
        {
            get
            {
                return false;
            }
        }

		protected int mCurrentCoroutineCount = 0;
        protected int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        protected LinkedList<IAsyncTask> mAsyncTaskStack;

		protected Dictionary<string,Assurer> mAssurerList;

        public override void Init()
		{
			mAsyncTaskStack = new LinkedList<IAsyncTask>();
			mAssurerList = new Dictionary<string, Assurer>();
		}

    #region  对外资源加载
        public virtual T LoadSync<T>(string AssetPath) where T : UnityEngine.Object
        {
			return null;
        }
        public virtual void LoadAsync<T>(string AssetPath,Action<T> finishCallback = null) where T:UnityEngine.Object
        {}
    #endregion


	#region  管理
        public virtual void PushInAsyncList(IAsyncTask task)
        {
            if (task == null)
            {
                Log.E("AsynTask is null!");
                return;
            }
            mAsyncTaskStack.AddLast(task);
            TryStartNextAsyncTask();
        }

        public virtual void PopUpAsyncList(IAsyncTask task)
        {
            if(task == null)
                return;
            mAsyncTaskStack.Remove(task);
        }

		protected void OnAsyncTaskFinish()
        {
            --mCurrentCoroutineCount;
            TryStartNextAsyncTask();
        }

        protected void TryStartNextAsyncTask()
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

        public virtual void MarkAssurer(Assurer assurer)
        {
            assurer.LoadSuccessCallback += OnAssurerLoaded;
            assurer.LoadFailCallback += OnAssurerLoadedFail;
        }

        public virtual void OnAssurerLoaded(Assurer assurer)
		{
            if(assurer.AutoRelease)
                assurer.Release();
		}
        public virtual void OnAssurerLoadedFail(Assurer assurer)
		{
            if(assurer.AutoRelease)
                assurer.ForceRecycle();
		}

        public virtual bool RemoveAssurer(Assurer assurer)
		{
            if(assurer == null || assurer.AssetPath.IsEmptyOrNull())
                return false;
			if(mAssurerList.ContainsKey(assurer.AssetPath))
			{
				mAssurerList.Remove(assurer.AssetPath);
				return true;
			}
			return true;	
		}

        public virtual void RecycleAssurer(Assurer assurer)
        {
            assurer.RecycleSelf();
        }

        public virtual void ReUseAssurer(Assurer assurer)
        {

        }
    #endregion


    #region Editor
        public virtual Dictionary<string,Assurer> GetAssurerList()
        {
            return mAssurerList;
        }
    #endregion
    }

}
