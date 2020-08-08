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

    public class NetAssetManager : VAssetManager<NetAssetManager>
	{
		public override string ManagerName
        {
            get
            {
                return "NetAssetManager";
            }
        }

		private Dictionary<string,NetAssurer> mNetAssurerList;
		public override void Init()
		{
            base.Init();
			mNetAssurerList = new Dictionary<string, NetAssurer>();
		}


    #region 对外方法
		///<summary>
		///因为是网络数据所以默认为异步
		///</summary>
		///<param name="bUnloadTag">释放模式</param>
		///<param name="bSave">下载完成后是否保存</param>
		///<param name="bLocalFirst">是否先加载本地</param>
        public NetAssurer GetNetAssurer<T>(string Url,Action<T> finishCallback = null,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false) where T : UnityEngine.Object
        {
           var assurer = Download(Url,typeof(T),(ar)=>{GetResOnFinish<T>(ar as NetAssurer,finishCallback);},bUnloadTag:bUnloadTag,bSave:bSave,bLocalFirst:bLocalFirst);
            if(assurer != null)
                assurer.AutoRelease = false;
            return assurer;
        }

        public void Download<T>(string Url,Action<T> finishCallback = null,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false) where T : UnityEngine.Object
        {
            Download(Url,typeof(T),(assurer)=>{GetResOnFinish<T>(assurer as NetAssurer,finishCallback);},bUnloadTag:bUnloadTag,bSave:bSave,bLocalFirst:bLocalFirst);
        }
    #endregion

    #region 核心
		///<param name="bUnloadTag">释放模式</param>
		///<param name="bSave">下载完成后是否保存</param>
		///<param name="bLocalFirst">是否先加载本地</param>
        private NetAssurer GetAssurer(string Url,Type AssetType,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false)
        {
			NetAssurer assurer = null;
            if(mNetAssurerList.ContainsKey(Url))
            {
                assurer = mNetAssurerList[Url];
                assurer.Init(Url,AssetType,bUnloadTag,bSave,bLocalFirst);
            }
            if(assurer == null)
            {
                assurer = NetAssurer.EasyGet();
                assurer.Init(Url,AssetType,bUnloadTag,bSave,bLocalFirst);
                mNetAssurerList.Add(assurer.AssetPath,assurer);
            }
            assurer.Retain();
            Log.IColor("NetAssurer[AssetPath:{0}]",LogColor.Blue,assurer.AssetPath);
            return assurer;
        }
    #endregion

	#region Async Load
       private void GetResOnFinish<T>(NetAssurer assurer,Action<T> callback) where T:UnityEngine.Object
        {
            if(callback == null)
                return;
            if(assurer == null)
                callback(null);
            callback(assurer.Get<T>());
        }
		protected NetAssurer Download(string Url,Type AssetType,Action<Assurer> finishCallback = null,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false)
        {
			var assurer = GetAssurer(Url,AssetType,bUnloadTag:bUnloadTag,bSave:bSave,bLocalFirst:bLocalFirst);
            if(finishCallback != null)
                assurer.LoadFinishCallback += finishCallback;
            assurer.Download();
            return assurer;
        }
	#endregion
	}
}

