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
		private Dictionary<string,ABAssurer> mAssurerList;

		public override void Init()
		{
			mAsyncTaskStack = new LinkedList<IAsyncTask>();
			mRealABFilePath = new Dictionary<string, string>();
			mAssurerList = new Dictionary<string, ABAssurer>();
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