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
		public static ABAssurer EasyGet()
		{
			var assurer = EasyPool<ABAssurer>.Instance.Get();
			return assurer;
		}

		private bool mFileExist = false;
		//资源释放模式
		public bool UnloadTag;
		//AB地址
		public string RealPath;
		//AB文件
		public string FileName;
		//依赖文件List
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
		private AssetLoadState mLoadState = AssetLoadState.None;

		public override float Process
		{
			get
			{
				if(mABCR == null)
					return 0;
				return mABCR.progress;
			}
		}

		public void Init(ABPathAnalysis analysis)
		{
			mAssetPath = analysis.AssetPath;
			mFileExist = analysis.B_FileExist;
			FileName = analysis.FileName;
			RealPath = analysis.RealPath;
			UnloadTag = analysis.B_UnloadTag;

			analysis.RecycleSelf();
			analysis = null;
		}

		protected override void Rest()
		{
			Log.I("[ABAssurer]{0}:RecycleSelf",AssetPath);
			base.Rest();
			if(mABCR != null && !mABCR.isDone)
				OnFail2Load();
			if(mAB != null)
				mAB.Unload(UnloadTag);
			mBinary = null;
			mAB = null;
			mABCR = null;
			RealPath = null;
			FileName = null;
			DependFileList = null;
			mLoadState = AssetLoadState.None;
			LoadFinishCallback = null;
		}

		public override T Get<T>()
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
			mLoadState = AssetLoadState.Done;
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
			if(mLoadState != AssetLoadState.None)
				return;
			mLoadState = AssetLoadState.Loading;
			ABManager.Instance.PushInAsyncList(this);
		}

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
			if(mLoadState != AssetLoadState.Loading)
			{
				Log.E("State isn`t AssetLoadState.Loading");
				finishCallback();
				yield break;
			}
			if(!mFileExist)
			{
				Log.E("File Not Exist:{0}",RealPath);
				mLoadState = AssetLoadState.Done;
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
				if (!mABCR.isDone || mLoadState != AssetLoadState.Loading)
				{
					Log.E("AssetBundleCreateRequest Not Done! Path:" + RealPath);
					mLoadState = AssetLoadState.Done;
					OnFail2Load();
					finishCallback();
					yield break;
				}
				mAB = mABCR.assetBundle;
			}
			mLoadState = AssetLoadState.Done;
			OnSuccess2Load();
			finishCallback();
        }

		public override void Recycle()
		{
			if(mUseCount > 0)
			{
				//TODO UnSafe Tip
			}
			ABManager.Instance.RemoveAssurer(this);
			Rest();
		}

		public void ForceRecycle()
		{
			mUseCount = 0;
			if(mLoadState == AssetLoadState.Wait4Recycle)
				return;
			mLoadState = AssetLoadState.Wait4Recycle;
			ABManager.Instance.WaitForRecycle(this);
		}

		public override void Retain()
		{
			base.Retain();
			if(mLoadState == AssetLoadState.Wait4Recycle)
			{
				ABManager.Instance.ReUseAssurer(this);
				if(mAB == null)
					mLoadState = AssetLoadState.None;
				else
					mLoadState = AssetLoadState.Done;
			}
		}

		protected override void Become2Useless()
		{
			Log.I("Become2Useless:{0}",AssetPath);

			if(mLoadState == AssetLoadState.Loading)
			{
				Log.E("Loading break off");
				mLoadState = AssetLoadState.Done;
				ABManager.Instance.PopUpAsyncList(this);
			}
			base.Become2Useless();
			mLoadState = AssetLoadState.Wait4Recycle;
			ABManager.Instance.WaitForRecycle(this);
		}
		protected override void OnSuccess2Load()
		{
			Log.I("OnSuccess2Load:{0}",AssetPath);

			if(LoadFinishCallback != null)
			{
				LoadFinishCallback.Invoke(this);
				LoadFinishCallback = null;
			}
			//TODO Release 的时机非常重要
			Release();
		}
		protected override void OnFail2Load()
		{
			Log.E("OnFail2Load:{0}",RealPath);

			if(LoadFinishCallback != null)
			{
				LoadFinishCallback.Invoke(null);
				LoadFinishCallback = null;
			}
			ForceRecycle();
		}
	}
}
