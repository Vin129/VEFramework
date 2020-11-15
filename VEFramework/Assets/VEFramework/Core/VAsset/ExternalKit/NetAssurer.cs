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
    using UnityEngine.Networking;

    public class NetAssurer : Assurer
    {
		public static NetAssurer EasyGet()
		{
			var assurer = EasyPool<NetAssurer>.Instance.Get();
			return assurer;
		}
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
		public override event Action<Assurer> LoadSuccessCallback;
		public override event Action<Assurer> LoadFailCallback;
		private bool mBSave = false;
		private Type mAssetType;
		private byte[] mBytesAsset;
		private string mTextAsset;
		private AssetBundle mABAsset;
		private UnityEngine.Object mAsset;
		private UnityWebRequest mWBER;

		public override float Progress
		{
			get
			{
				if(mWBER == null)
					return 0;
				return mWBER.downloadProgress;
			}
		}

		///<param name="bUnloadTag">释放模式</param>
		///<param name="bSave">下载完成后是否保存</param>
		///<param name="bLocalFirst">是否先加载本地</param>
		public void Init(string AssetPath,Type AssetType,bool bUnloadTag = false,bool bSave = false,bool bLocalFirst = false)
		{
			if(bLocalFirst)
			{
				string localPath = string.Empty;
				if(PathUtil.ExternalAssetExist(AssetPath,ref localPath))
					mAssetPath = "file://" + localPath;
				else
					mAssetPath = AssetPath;
			}
			else
			{
				mAssetPath = AssetPath;
			}
			mAssetType = AssetType;
			mBSave = bSave;
			UnloadTag = bUnloadTag;
		}

		protected override void Reset()
		{
			LogIColor("[NetAssurer]{0}:RecycleSelf",LogColor.Blue,AssetPath);
			base.Reset();
			if(mWBER != null && !mWBER.isDone)
			{
				ErrorMessage = "UnityWebRequest has not Done";
				OnFail2Load();
			}
			if(mAsset != null && UnloadTag)
				Resources.UnloadAsset(mAsset);
			mBSave = false;
			mAssetType = null;
			mAsset = null;
			mBytesAsset = null;
			mTextAsset = null;
			mABAsset = null;
			if(mWBER != null)
				mWBER.Dispose();
			mWBER = null;
			LoadFinishCallback = null;
		}

		///</summary>
		///网络资产建议封装成AssetBundle 或自行修改此处进行处理mBytesAsset
		///</summary>
		public override T Get<T>()
		{
			try
			{
				var type = typeof(T);
				if(type == typeof(AssetBundle))
				{
					return mABAsset as T;
				}
				else if (type == typeof(AudioClip))
				{
					return mAsset as T;
				}
				else if (type == typeof(Texture2D))
				{
					return mAsset as T;
				}
				else if (type == typeof(TextAsset))
				{
					return mTextAsset as T;
				}
			}
			catch(Exception e)
			{
				ErrorMessage = e.ToString();
			}
			if(Error)
			{
				Log.E(ErrorMessage);
				return null;
			}
			return null;
		}

		public byte[] Get()
		{
			if(mBytesAsset != null)
				return mBytesAsset;
			return null;
		}

		public void Download()
		{
			if(mLoadState == AssetLoadState.Done)
			{
				if(Error)
					OnFail2Load();
				else
					OnSuccess2Load();
				return;
			}
			if(mLoadState != AssetLoadState.None)
				return;
			if(mAssetType == typeof(AssetBundle))
			{
				mWBER = UnityWebRequestAssetBundle.GetAssetBundle(AssetPath);
			}
            else if (mAssetType == typeof(AudioClip))
            {
                mWBER = UnityWebRequestMultimedia.GetAudioClip(AssetPath,AudioType.WAV);
            }
            else if (mAssetType == typeof(Texture2D))
            {
                mWBER = UnityWebRequestTexture.GetTexture(AssetPath);
            }
            else
            {
                mWBER = new UnityWebRequest(AssetPath);
                mWBER.downloadHandler = new DownloadHandlerBuffer();
            }
            mWBER.SendWebRequest();
			mLoadState = AssetLoadState.Loading;
			NetAssetManager.Instance.PushInAsyncList(this);
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
				yield return mWBER;
				yield return mWBER.downloadHandler;
				while(!mWBER.isDone)
				{
					if (mWBER.isNetworkError || mLoadState != AssetLoadState.Loading)
					{
						ErrorMessage = "AssetBundleCreateRequest Not Done! Path:" + AssetPath;
						mLoadState = AssetLoadState.Done;
						OnFail2Load();
						finishCallback();
						yield break;
					}
					yield return null;
				}
				if(mWBER.downloadHandler != null)
					mBytesAsset = mWBER.downloadHandler.data;
				if (mAssetType != typeof(Texture2D))
				{
					if (mAssetType != typeof(TextAsset))
					{
						if (mAssetType != typeof(AudioClip))
						{
							if(mAssetType == typeof(AssetBundle))
							{
								mABAsset = DownloadHandlerAssetBundle.GetContent(mWBER);
							}
						}
						else
						{
							mAsset = DownloadHandlerAudioClip.GetContent(mWBER);
						}
					}
					else
					{
						mTextAsset = mWBER.downloadHandler.text;
					}
					if(mBSave && mWBER.downloadHandler != null && mWBER.downloadHandler.data != null)
						PathUtil.SaveExternalAsset(AssetPath,mWBER.downloadHandler.data);
				}
				else
				{
					mAsset = DownloadHandlerTexture.GetContent(mWBER);
					if(mBSave)
						PathUtil.SaveExternalAsset(AssetPath,(mAsset as Texture2D).EncodeToPNG());
				}
			}
			mLoadState = AssetLoadState.Done;
			OnSuccess2Load();
			finishCallback();
        }

		public override void Recycle()
		{
			if(mUseCount > 0)
			{
				Log.EColor("UnSaveRecycle:{0}[AssetPath:{1}]",LogColor.ErrorTipLv1,mUseCount,AssetPath);
			}
			NetAssetManager.Instance.RemoveAssurer(this);
			Reset();
		}

		public override void ForceRecycle()
		{
			mUseCount = 0;
			if(mLoadState == AssetLoadState.Wait4Recycle)
				return;
			mLoadState = AssetLoadState.Wait4Recycle;
			NetAssetManager.Instance.RecycleAssurer(this);
		}

		public override void Retain()
		{
			base.Retain();
			if(mLoadState == AssetLoadState.Wait4Recycle)
			{
				NetAssetManager.Instance.ReUseAssurer(this);
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
				NetAssetManager.Instance.PopUpAsyncList(this);
			}
			base.Become2Useless();
			mLoadState = AssetLoadState.Wait4Recycle;
			NetAssetManager.Instance.RecycleAssurer(this);
		}
		protected override void OnSuccess2Load()
		{
			Log.I("OnSuccess2Load:{0}",AssetPath);

			if(LoadFinishCallback != null)
			{
				LoadFinishCallback.Invoke(this);
				LoadFinishCallback = null;
			}
			if(LoadSuccessCallback != null)
			{
				LoadSuccessCallback.Invoke(this);
			}
		}
		protected override void OnFail2Load()
		{
			Log.E("OnFail2Load:{0}",AssetPath);
			Log.E(ErrorMessage);
			if(LoadFinishCallback != null)
			{
				LoadFinishCallback.Invoke(null);
				LoadFinishCallback = null;
			}
			if(LoadFailCallback != null)
			{
				LoadFailCallback.Invoke(this);
			}
		}
	}

}

