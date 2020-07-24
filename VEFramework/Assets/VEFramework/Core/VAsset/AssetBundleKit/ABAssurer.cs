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
    using System.Collections;
    using UnityEngine;
    public class ABAssurer : Assurer
    {
		public static ABAssurer EasyGet(string assetPath)
		{
			var assurer = EasyPool<ABAssurer>.Instance.Get();
			assurer.AssetPath = assetPath;
			assurer.Init();
			return assurer;
		}

		private AssetBundle mAB;
		public AssetBundle AB 
		{
			get{return mAB;}
		}
		private byte[] mBinary;
		private AssetBundleCreateRequest mABCR;

		public float Process
		{
			get
			{
				if(mABCR == null)
					return 0;
				return mABCR.progress;
			}
		}

		public ABAssurer(){}
		public ABAssurer(string path) : this(path,false){}
		public ABAssurer(string path,bool AsyncMode)
		{
			AssetPath = path;
			mAsyncMode = AsyncMode;
			Init();	
		}

		private void Init()
		{
			InUse();
		}

		public override bool LoadSync()
		{
			mAB = AssetBundle.LoadFromFile(AssetPath);
			return DoLoadAsync();
		}

		public override bool LoadSync(byte[] binary)
		{
			mAB = AssetBundle.LoadFromMemory(binary);
			return DoLoadAsync();
		}

		public bool DoLoadAsync()
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
			//TODO Add List
		}

		public override void LoadAsync(byte[] binary)
		{
			mBinary = binary;
			//TODO Add List
		}

		public IEnumerator DoLoadAsync4Memory(System.Action finishCallback)
		{
			mABCR = AssetBundle.LoadFromMemoryAsync(mBinary);
			yield return mABCR;
			if (!mABCR.isDone)
			{
				// Log.E("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
				OnFail2Load();
				finishCallback();
				yield break;
			}
			mAB = mABCR.assetBundle;
			OnSuccess2Load();
			finishCallback();
		}

        public override IEnumerator DoLoadAsync(System.Action finishCallback)
        {
			mABCR = AssetBundle.LoadFromFileAsync(AssetPath);
			yield return mABCR;
			if (!mABCR.isDone)
			{
				// Log.E("AssetBundleCreateRequest Not Done! Path:" + mAssetName);
				OnFail2Load();
				finishCallback();
				yield break;
			}
			mAB = mABCR.assetBundle;
			OnSuccess2Load();
			finishCallback();
        }

		public override void Recycle()
		{
			if(mUseCount > 0)
			{
				//TODO UnSafe Tip
			}
			base.Recycle();
			if(mABCR != null && !mABCR.isDone)
				OnFail2Load();
			if(mAB != null)
				mAB.Unload(true);
			mAB = null;
			mABCR = null;
		}

		protected override void Become2Useless()
		{
			//TODO Wait4Recycle
		}
		protected override void OnSuccess2Load()
		{
			
		}
		protected override void OnFail2Load()
		{
			
		}
	}
}
