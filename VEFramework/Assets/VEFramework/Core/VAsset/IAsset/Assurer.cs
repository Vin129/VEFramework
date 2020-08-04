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
    //资产承保者：资产存放的最小单位
    public abstract class Assurer : IAsset,IAsyncTask,ICounter, IReusable
    {
		protected string mAssetPath = string.Empty;
        public abstract string AssetPath {get;set;}

        public float KeepTime = AssetCustomSetting.AssetKeepTime;

		protected int mUseCount = 0;
        public int UseCount
        {
            get
            {
                return mUseCount;
            }

            set
            {
				mUseCount = value;
            }
        }

        public virtual bool LoadSync(){return true;}
        public virtual bool LoadSync(byte[] binary){return true;}
        public virtual void LoadAsync(){}
        public virtual void LoadAsync(byte[] binary){}
        public virtual IEnumerator DoLoadAsync(System.Action finishCallback){ finishCallback(); yield break;}
       
       
        public virtual void Retain()
        {
            mUseCount++;
        }
        public virtual void Release()
        {
            mUseCount--;
            if(mUseCount <= 0)
                Become2Useless();
        }
		public virtual void Reset()
        {
            mUseCount = 0;
        }

        public virtual void Recycle()
        {

        }
        protected virtual void Rest()
        {
            mAssetPath = null;
			mUseCount = 0;
        }
        public virtual void Reuse(){}
        protected virtual void Become2Useless()
        {
            KeepTime = AssetCustomSetting.AssetKeepTime;
        }
        protected virtual void OnSuccess2Load(){}
        protected virtual void OnFail2Load(){}
    }
}
