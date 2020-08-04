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
    using System;
    using System.Collections;

    public class VAssetManager : VEManagers<VAssetManager>,IAsyncTaskContainer
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
		public override void Init()
		{
			mAsyncTaskStack = new LinkedList<IAsyncTask>();
			mAssurerList = new Dictionary<string, Assurer>();
		}

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
	}
    #endregion
}
