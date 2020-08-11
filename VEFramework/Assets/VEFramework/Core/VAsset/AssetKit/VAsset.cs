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
    using System.Collections;
    using System.Collections.Generic;

    public class VAsset : VEManagers<VAsset>
    {
        public override string ManagerName
        {
            get
            {
                return "VAsset";
            }
        }

		public override void Init()
		{
			if(AppSetting.AssetBundleOpen)
				ABManager.Instance.DoInit();
			ResManager.Instance.DoInit();
		}


	#region 一般资源加载接口
		public T LoadSync<T>(string AssetPath) where T : UnityEngine.Object
        {
			T kObj = null;
			if(AppSetting.AssetBundleOpen)
				kObj = ABManager.Instance.LoadSync<T>(AssetPath);
			if(kObj == null)
				kObj = ResManager.Instance.LoadSync<T>(AssetPath);
			return kObj;
        }
        public void LoadAsync<T>(string AssetPath,Action<T> finishCallback = null) where T:UnityEngine.Object
        {
			if(AppSetting.AssetBundleOpen)
				ABManager.Instance.LoadAsync<T>(AssetPath,finishCallback);
			else
				ResManager.Instance.LoadAsync<T>(AssetPath,finishCallback);
		}

		public Assurer GetAssurerSync(string AssetPath)
		{
			if(AppSetting.AssetBundleOpen)
				return ABManager.Instance.GetAssurerSync(AssetPath);
			else
				return ResManager.Instance.GetAssurerSync(AssetPath); 
		}
		public Assurer GetAssurerAsync(string AssetPath) 
		{
			if(AppSetting.AssetBundleOpen)
				return ABManager.Instance.GetAssurerAsync(AssetPath);
			else
				return ResManager.Instance.GetAssurerAsync(AssetPath); 
		}
		public Assurer GetAssurerAsync<T>(string AssetPath,Action<T> finishCallback = null) where T :UnityEngine.Object
		{
			if(AppSetting.AssetBundleOpen)
				return ABManager.Instance.GetAssurerAsync(AssetPath,finishCallback);
			else
				return ResManager.Instance.GetAssurerAsync(AssetPath,finishCallback); 
		}

	#endregion


	#region 适合扩展修改的加载接口
	
	///<summary>
	///下载网络资产
	///</summary>
	public void DownloadAsset<T>(string Url,Action<T> finishCallback = null,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false) where T : UnityEngine.Object
	{
        NetAssetManager.Instance.Download<T>(Url,finishCallback,bUnloadTag:bUnloadTag,bSave:bSave,bLocalFirst:bLocalFirst);
    }
	///<summary>
	///下载网络资产 byte
	///</summary>
	public void DownloadAsset(string Url,Action<byte[]> finishCallback = null,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false)
	{
        NetAssetManager.Instance.Download(Url,finishCallback,bUnloadTag:bUnloadTag,bSave:bSave,bLocalFirst:bLocalFirst);
    }


	#endregion




	#region 辅助方法
		private void GetAssetOnFinish<T>(Assurer assurer,Action<T> callback) where T:UnityEngine.Object
        {
            if(callback == null)
                return;
            if(assurer == null)
                callback(null);
            callback(assurer.Get<T>());
        }

	#endregion




    }
}
