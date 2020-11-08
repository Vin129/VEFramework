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

		public override void Init()
		{
            base.Init();
		}


    #region 对外方法
        public ResAssurer GetAssurerSync(string AssetPath)
        {
            var assurer = LoadSync(AssetPath);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }
        public ResAssurer GetAssurerAsync(string AssetPath)
        {
           var assurer = LoadAsync(AssetPath);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }
        public ResAssurer GetAssurerAsync<T>(string AssetPath,Action<T> finishCallback = null)where T:UnityEngine.Object
        {
           var assurer = LoadAsync(AssetPath,(ar)=>{GetResOnFinish<T>(ar as ResAssurer,finishCallback);});
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }

        public override T LoadSync<T>(string AssetPath)
        {
			var assurer = LoadSync(AssetPath);
            return assurer.Get<T>();
        }
        public override void LoadAsync<T>(string AssetPath,Action<T> finishCallback = null) 
        {
            LoadAsync(AssetPath,(assurer)=>{GetResOnFinish<T>(assurer as ResAssurer,finishCallback);});
        }
    #endregion

    #region 核心
        private ResAssurer GetAssurer(string AssetPath,bool bUnloadTag)
        {
			ResAssurer assurer = null;
            if(mAssurerList.ContainsKey(AssetPath))
            {
                assurer = mAssurerList[AssetPath] as ResAssurer;
                assurer.Init(AssetPath,bUnloadTag);
            }
            if(assurer == null)
            {
                assurer = ResAssurer.EasyGet();
                MarkAssurer(assurer);
                assurer.Init(AssetPath,bUnloadTag);
                mAssurerList.Add(assurer.AssetPath,assurer);
            }
            assurer.Retain();
            Log.IColor("ResAssurer[AssetPath:{0}]",LogColor.Orange,assurer.AssetPath);
            return assurer;
        }
    #endregion

 	#region Sync Load
        protected ResAssurer LoadSync(string AssetPath)
        {
			var assurer = GetAssurer(AssetPath,DefaultUnLoadTag);
			assurer.LoadSync();
            return assurer;
        }
	#endregion

	#region Async Load
       private void GetResOnFinish<T>(ResAssurer assurer,Action<T> callback) where T:UnityEngine.Object
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
		protected ResAssurer LoadAsync(string AssetPath,Action<Assurer> finishCallback = null)
        {
			var assurer = GetAssurer(AssetPath,DefaultUnLoadTag);
            if(finishCallback != null)
                assurer.LoadFinishCallback += finishCallback;
            assurer.LoadAsync();
            return assurer;
        }
	#endregion
	}
}
