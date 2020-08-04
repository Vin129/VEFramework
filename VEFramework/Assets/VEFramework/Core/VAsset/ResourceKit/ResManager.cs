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

    public class ResManager : VAssetManager<ResManager>
	{
		public override string ManagerName
        {
            get
            {
                return "ResManager";
            }
        }

		private Dictionary<string,ResAssurer> mAssurerList;
		public override void Init()
		{
            base.Init();
			mAssurerList = new Dictionary<string, ResAssurer>();
		}

        private ResAssurer GetAssurer(string AssetPath,bool bUnloadTag = false)
        {
			ResAssurer assurer = null;
            if(mAssurerList.ContainsKey(AssetPath))
            {
                assurer = mAssurerList[AssetPath];
                assurer.Init(AssetPath,bUnloadTag);
            }
            if(assurer == null)
            {
                assurer = ResAssurer.EasyGet();
                assurer.Init(AssetPath,bUnloadTag);
                mAssurerList.Add(assurer.AssetPath,assurer);
            }
            assurer.Retain();
            Log.I("Assurer[AssetPath:{0}]",assurer.AssetPath);
            return assurer;
        }

 	#region Sync Load
        public ResAssurer LoadSync(string AssetPath)
        {
			var assurer = GetAssurer(AssetPath);
			assurer.LoadSync();
            return assurer;
        }

        public T LoadSync<T>(string AssetPath) where T : UnityEngine.Object
        {
			var assurer = LoadSync(AssetPath);
            return assurer.Get<T>();
        }
	#endregion

	#region Async Load
       private void GetResOnFinish<T>(ResAssurer assurer,Action<T> callback) where T:UnityEngine.Object
        {
            if(callback == null)
                return;
            if(assurer == null)
                callback(null);
            callback(assurer.Get<T>());
        }
        public void LoadAsync<T>(string AssetPath,Action<T> finishCallback = null) where T:UnityEngine.Object
        {
            LoadAsync(AssetPath,(assurer)=>{GetResOnFinish<T>(assurer,finishCallback);});
        }
		protected void LoadAsync(string AssetPath,Action<ResAssurer> finishCallback = null)
        {
			var assurer = GetAssurer(AssetPath);
            if(finishCallback != null)
                assurer.LoadFinishCallback += finishCallback;
            assurer.LoadAsync();
        }
	#endregion
	}
}
