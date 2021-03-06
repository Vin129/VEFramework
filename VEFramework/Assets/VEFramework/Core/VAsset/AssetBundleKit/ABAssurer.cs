﻿/****************************************************************************
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
    using UnityEngine;
    public class ABAssurer : Assurer
    {
		public static ABAssurer EasyGet()
		{
			var assurer = EasyPool<ABAssurer>.Instance.Get();
			return assurer;
		}

		private bool mFileExist = false;
		//AB地址
		public string RealPath;
		//AB文件
		public string FileName;
		//依赖加载
		public bool DepLoad = false;
		//依赖文件List
		public string[] DependFileList;
		public List<ABAssurer> DependAssurerList;
		public override event Action<Assurer> LoadFinishCallback;
		public override event Action<Assurer> LoadSuccessCallback;
		public override event Action<Assurer> LoadFailCallback;
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

		public override float Progress
		{
			get
			{
				if(mLoadState == AssetLoadState.None)
					return 0;
				else if(mLoadState == AssetLoadState.Loading)
				{
					float tempValue = 0; 
					int count = 1;
					if(DependAssurerList != null)
					{
						DependAssurerList.ForEach(assurer=>{tempValue += assurer.Progress;});
						count += DependAssurerList.Count;
					}
					if(mABCR == null)
						return tempValue/count;
					return (tempValue + mABCR.progress)/count;
				} 
				else
				{
					return 1;
				}
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

		public override void Release()
        {
			ABManager.Instance.ReleaseDepend(DependFileList);
            base.Release();
        }

		public override void Release(bool releaseMode)
        {
			ABManager.Instance.ReleaseDepend(DependFileList,releaseMode);
            base.Release(releaseMode);
        }

		protected override void Reset()
		{
			LogIColor("[ABAssurer]{0}:RecycleSelf {1}",LogColor.Green,AssetPath,UnloadTag);
			base.Reset();
			if(mABCR != null && !mABCR.isDone)
			{
				ErrorMessage = "AssetBundleCreateRequest has not Done";
				OnFail2Load();
			}
			if(mAB != null)
				ABManager.Instance.UnloadAsset(mAB,UnloadTag);
			mBinary = null;
			mAB = null;
			mABCR = null;
			RealPath = null;
			FileName = null;
			DepLoad = false;
			DependFileList = null;
			if(DependAssurerList != null)
			{
				DependAssurerList.Clear();
				DependAssurerList = null;
			}
			LoadFinishCallback = null;
		}

		protected bool IsContains(string assetsName,string targetName)
		{
			return assetsName.Contains(targetName);
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
				if(IsContains(assetsNames[i],FileName))
				{
					LogIColor("Get:{0}",LogColor.Green,assetsNames[i]);
					string strVal = assetsNames[i].Substring(assetsNames[i].IndexOf(FileName));
					string check = assetsNames[i].Substring(0,assetsNames[i].IndexOf(FileName));
					int iIdx = strVal.LastIndexOf(".");
					if(-1 != iIdx)
						strVal = strVal.Substring(0,iIdx);
					if(strVal == FileName && (check.IsEmptyOrNull()||check.EndsWith("/")))
					{
						if(typeof(T) == typeof(GameObject))
						{
							var kObj = GameObject.Instantiate(mAB.LoadAsset<T>(assetsNames[i]));
							if(kObj != null)
								kObj.name = kObj.name.Replace("(Clone)","");
							return kObj;
						}
						else
						{
							return mAB.LoadAsset<T>(assetsNames[i]);
						}		
					}		
				}
        	}
			Log.E("No Get:{0}",FileName);
			return null;
		}

		public override T Get<T>(string FileName)
		{
			this.FileName = FileName;
			return Get<T>();
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
				ErrorMessage = string.Format("File Not Exist:{0}",RealPath);
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
				ErrorMessage = "AssetBundle is Null!";
				OnFail2Load();
				return false;
			}
			OnSuccess2Load();
			return true;
		}

		public override void LoadAsync()
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
				ErrorMessage = string.Format("File Not Exist:{0}",RealPath);
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
					ErrorMessage = "AssetBundleCreateRequest Not Done! Path:" + RealPath;
					mLoadState = AssetLoadState.Done;
					OnFail2Load();
					finishCallback();
					yield break;
				}
				if(DependAssurerList != null)
				{
					while(DependAssurerList.Count > 0)
					{
						for(int i = 0;i<DependAssurerList.Count;i++)
						{
							if(DependAssurerList[i].Progress >= 1)
							{
								DependAssurerList.RemoveAt(i);
								i--;
							}
						}
						yield return null;
					}
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
				Log.EColor("UnSaveRecycle:{0}[AssetPath:{1},RealPath:{2},FileName:{3}]",LogColor.ErrorTipLv1,mUseCount,AssetPath,RealPath,FileName);
			}
			ABManager.Instance.RemoveAssurer(this);
			Reset();
		}

		public override void ForceRecycle()
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
			if(LoadSuccessCallback != null)
			{
				LoadSuccessCallback.Invoke(this);
			}
		}
		protected override void OnFail2Load()
		{
			Log.E("OnFail2Load:{0}",RealPath);
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
