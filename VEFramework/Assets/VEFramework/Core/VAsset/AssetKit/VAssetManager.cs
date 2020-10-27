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
		protected int mCurrentCoroutineCount = 0;
        protected int mMaxCoroutineCount = 8; //最快协成大概在6到8之间
        protected LinkedList<IAsyncTask> mAsyncTaskStack;

		private Dictionary<string,Assurer> mAssurerList;

        public virtual event Action<Assurer> InitiativeRecycleAction;
        public virtual event Action<Assurer> InitiativeReUseAction;

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


        public virtual bool RemoveAssurer(Assurer aber)
		{
            if(aber == null || aber.AssetPath.IsEmptyOrNull())
                return false;
			if(mAssurerList.ContainsKey(aber.AssetPath))
			{
				mAssurerList.Remove(aber.AssetPath);
				return true;
			}
			return true;	
		}

        public virtual void RecycleAssurer(Assurer aber)
        {
            if(AssetCustomSetting.AssetUnLoadMode == AssetUnLoadModeType.I_DONT_CARE)
            {
                aber.RecycleSelf();
            } 
            else if(AssetCustomSetting.AssetUnLoadMode == AssetUnLoadModeType.BEGIN_AND_END)
            {
                InitiativeRecycleAction.Invoke(aber);
            }
        }

        public virtual void ReUseAssurer(Assurer aber)
        {
            if(AssetCustomSetting.AssetUnLoadMode == AssetUnLoadModeType.I_DONT_CARE)
            {
                
            } 
            else if(AssetCustomSetting.AssetUnLoadMode == AssetUnLoadModeType.BEGIN_AND_END)
            {
                InitiativeReUseAction.Invoke(aber);
            }
        }
    }
    #endregion
}
