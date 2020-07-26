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
    using UnityEngine;
    public class ABAssurer : Assurer
    {
		public static ABAssurer EasyGet(string assetPath,bool bPostfix = true)
		{
			var assurer = EasyPool<ABAssurer>.Instance.Get();
			assurer.mAssetPath = assetPath;
			assurer.InitAssetPath(bPostfix);
			assurer.Init(); 
			return assurer;
		}

		private bool mFileExist = false;
		public string RealPath;
		public string FileName;
		public string[] DependFileList;
		public event Action<ABAssurer> LoadFinishCallback;
		public override string AssetPath
		{
			get
			{
				return mAssetPath;
			}
			set
			{
				mAssetPath = value;
			}
		}


		private AssetBundle mAB;
		public AssetBundle AB 
		{
			get{return mAB;}
		}
		private byte[] mBinary;
		private AssetBundleCreateRequest mABCR;
		private AsyncLoadState mAsyncState = AsyncLoadState.None;


		public float Process
		{
			get
			{
				if(mABCR == null)
					return 0;
				return mABCR.progress;
			}
		}

		private void Init()
		{

		}

		private void InitAssetPath(bool bPostfix)
		{
			FileName = string.Empty;
			RealPath = ABManager.Instance.GetAssetbundleRealPath(mAssetPath,ref mFileExist,ref FileName,bPostfix);
		}

		protected override void Rest()
		{
			base.Rest();
			if(mABCR != null && !mABCR.isDone)
				OnFail2Load();
			if(mAB != null)
				mAB.Unload(false);
			mBinary = null;
			mAB = null;
			mABCR = null;
			RealPath = null;
			FileName = null;
			DependFileList = null;
			mAsyncState = AsyncLoadState.None;
			LoadFinishCallback = null;
		}

		public T Get<T>() where T:UnityEngine.Object
		{
			if(mAB == null || FileName.IsEmptyOrNull())
				return null;
			if(typeof(T) == typeof(AssetBundle))
				return mAB as T;
			FileName = FileName.ToLower();
			string[] assetsNames = mAB.GetAllAssetNames();
			for(int i = 0;i < assetsNames.Length;i++)
			{
				if(assetsNames[i].Contains(FileName))
				{
					Log.I("Get:{0}",assetsNames[i]);
					string strVal = assetsNames[i].Substring(assetsNames[i].IndexOf(FileName));
					int iIdx = strVal.LastIndexOf(".");
					if(-1 != iIdx)
						strVal = strVal.Substring(0,iIdx);
					if(strVal == FileName)
						return mAB.LoadAsset<T>(assetsNames[i]);
				}
        	}
			Log.E("No Get:{0}",FileName);
			return null;
		}

		public T LoadSync<T>() where T : UnityEngine.Object
		{
			if(!LoadSync())
				return null;
			return Get<T>();
		}

		public override bool LoadSync()
		{
			if(!mFileExist)
			{
				Log.E("File Not Exist:{0}",RealPath);
				OnFail2Load();
				return false;
			}
			if(mAB == null)
			{
				if(mBinary == null)
					mAB = AssetBundle.LoadFromFile(RealPath);
				else
					mAB = AssetBundle.LoadFromMemory(mBinary);
			}
			return DoLoadSync();
		}

		public bool DoLoadSync()
		{
			if(mAB == null)
			{
				OnFail2Load();
				return false;
			}
			OnSuccess2Load();
			return true;
		}

		public override void LoadAsync()
		{
			if(mAsyncState != AsyncLoadState.None)
				return;
			ABManager.Instance.PushInAsyncList(this);
			mAsyncState = AsyncLoadState.Loading;
		}

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
			if(!mFileExist)
			{
				Log.E("File Not Exist:{0}",RealPath);
				mAsyncState = AsyncLoadState.Done;
				OnFail2Load();
				finishCallback();
				yield break;
			}

			if(mAB == null)
			{
				if(mBinary == null)
					mABCR = AssetBundle.LoadFromFileAsync(RealPath);
				else
					mABCR = AssetBundle.LoadFromMemoryAsync(mBinary);
				yield return mABCR;
				if (!mABCR.isDone)
				{
					Log.E("AssetBundleCreateRequest Not Done! Path:" + RealPath);
					mAsyncState = AsyncLoadState.Done;
					OnFail2Load();
					finishCallback();
					yield break;
				}
				mAB = mABCR.assetBundle;
			}
			mAsyncState = AsyncLoadState.Done;
			OnSuccess2Load();
			finishCallback();
        }

		public override void Recycle()
		{
			if(mUseCount > 0)
			{
				//TODO UnSafe Tip
			}
			Rest();
		}

		public void ForceRecycle()
		{
			ABManager.Instance.RemoveAssurer(this);
			EasyPool<ABAssurer>.Instance.Recycle(this);
		}

		protected override void Become2Useless()
		{
			Log.I("Become2Useless:{0}",AssetPath);
			ABManager.Instance.RemoveAssurer(this);
			EasyPool<ABAssurer>.Instance.Recycle(this);
		}
		protected override void OnSuccess2Load()
		{
			Log.I("OnSuccess2Load:{0}",AssetPath);
			if(LoadFinishCallback != null)
				LoadFinishCallback.Invoke(this);
			//TODO add wait4recycle
		}
		protected override void OnFail2Load()
		{
			Log.E("OnFail2Load:{0}",RealPath);
			if(LoadFinishCallback != null)
				LoadFinishCallback.Invoke(null);
			ForceRecycle();
		}
	}
}
