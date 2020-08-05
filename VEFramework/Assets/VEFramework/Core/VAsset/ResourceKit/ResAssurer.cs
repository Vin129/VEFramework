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
    public class ResAssurer : Assurer
    {
		public static ResAssurer EasyGet()
		{
			var assurer = EasyPool<ResAssurer>.Instance.Get();
			return assurer;
		}
		//资源释放模式
		public bool UnloadTag;
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
		public override event Action<Assurer> LoadFinishCallback;
		private UnityEngine.Object mAsset;
		private ResourceRequest mRESR;

		public override float Process
		{
			get
			{
				if(mRESR == null)
					return 0;
				return mRESR.progress;
			}
		}

		public void Init(string AssetPath,bool bUnloadTag)
		{
			mAssetPath = AssetPath;
			UnloadTag = bUnloadTag;
		}

		protected override void Rest()
		{
			Log.I("[ResAssurer]{0}:RecycleSelf",AssetPath);
			base.Rest();
			if(mRESR != null && !mRESR.isDone)
				OnFail2Load();
			if(mAsset != null && UnloadTag)
				Resources.UnloadAsset(mAsset);
			mAsset = null;
			mRESR = null;
			LoadFinishCallback = null;
		}

		public override T Get<T>()
		{
			if(mAsset != null)
				return mAsset as T;
			Log.E("Asset Not Exist:{0}",AssetPath);
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
			mAsset = Resources.Load(AssetPath);
			return DoLoadSync();
		}

		public bool DoLoadSync()
		{
			if(mAsset == null)
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
			ResManager.Instance.PushInAsyncList(this);
		}

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
			if(mLoadState != AssetLoadState.Loading)
			{
				Log.E("State isn`t AssetLoadState.Loading");
				finishCallback();
				yield break;
			}
			if(mAsset == null)
			{
				mRESR = Resources.LoadAsync(AssetPath);
				yield return mRESR;
				if (!mRESR.isDone || mLoadState != AssetLoadState.Loading)
				{
					Log.E("AssetBundleCreateRequest Not Done! Path:" + AssetPath);
					mLoadState = AssetLoadState.Done;
					OnFail2Load();
					finishCallback();
					yield break;
				}
				mAsset = mRESR.asset;
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
			ResManager.Instance.RemoveAssurer(this);
			Rest();
		}

		public void ForceRecycle()
		{
			mUseCount = 0;
			if(mLoadState == AssetLoadState.Wait4Recycle)
				return;
			mLoadState = AssetLoadState.Wait4Recycle;
			ResManager.Instance.RecycleAssurer(this);
		}

		public override void Retain()
		{
			base.Retain();
			if(mLoadState == AssetLoadState.Wait4Recycle)
			{
				ResManager.Instance.ReUseAssurer(this);
				if(mAsset == null)
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
				ResManager.Instance.PopUpAsyncList(this);
			}
			base.Become2Useless();
			mLoadState = AssetLoadState.Wait4Recycle;
			ResManager.Instance.RecycleAssurer(this);
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
			Log.E("OnFail2Load:{0}",AssetPath);

			if(LoadFinishCallback != null)
			{
				LoadFinishCallback.Invoke(null);
				LoadFinishCallback = null;
			}
			ForceRecycle();
		}
	}

}
