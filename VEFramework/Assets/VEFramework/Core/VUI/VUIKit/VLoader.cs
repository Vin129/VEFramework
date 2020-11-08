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
    public class VLoader :IAssurerLoader,IReusable
    {
        public static VLoader EasyGet()
		{
			var loader = EasyPool<VLoader>.Instance.Get();
			return loader;
		}


        private Dictionary<string,Assurer> mLoadedAssurer;
        public VLoader()
        {
            Init();
        }
        protected void Init()
        {
            mLoadedAssurer = new Dictionary<string, Assurer>();
        }


#region 加载方法
        public T LoadSync<T>(string AssetPath) where T:UnityEngine.Object
        {
            if(AssetPath.IsEmptyOrNull())
                return null;
            Assurer assurer = ContainsAssetPath(AssetPath)?mLoadedAssurer[AssetPath]:GetAssurer(AssetPath);
            if(assurer == null)
                return null;
            return assurer.Get<T>();
        }


        public void LoadAsync<T>(string AssetPath,Action<T> FinishCallback = null) where T:UnityEngine.Object
        {
            if(AssetPath.IsEmptyOrNull())
                return;
            if(ContainsAssetPath(AssetPath))
            {
                if(FinishCallback != null)
                    FinishCallback.Invoke(mLoadedAssurer[AssetPath].Get<T>());
            }
            else
                GetAssurerAsync(AssetPath,FinishCallback);
        }
#endregion

        protected Assurer GetAssurer(string AssetPath)
        {
            var assurer = VAsset.Instance.GetAssurerSync(AssetPath);
            if(assurer != null)
                mLoadedAssurer.Add(AssetPath,assurer);
            return assurer;
        }

        protected void GetAssurerAsync<T>(string AssetPath,Action<T> FinishCallback = null) where T:UnityEngine.Object
        {
            var assurer = VAsset.Instance.GetAssurerAsync<T>(AssetPath,FinishCallback);
            if(assurer != null)
                mLoadedAssurer.Add(AssetPath,assurer);
        }

        protected bool ContainsAssetPath(string AssetPath)
        {
            if(mLoadedAssurer.ContainsKey(AssetPath))
            {
                if(mLoadedAssurer[AssetPath].Error)
                {
                    Log.E("AssurerError:{0}[AssetPath:{1}]",mLoadedAssurer[AssetPath].ErrorMessage,AssetPath);
                    mLoadedAssurer.Remove(AssetPath);
                    return false;
                }
                return true;
            }
            return false;
        }

        //TODO:Release 与 VAsset 直接的联系不健康
        //TODO:梳理清楚 ReleaseMode UnLoadTag AutoRelease 关系 
        public void Release(bool ReleaseMode)
        {
            mLoadedAssurer.ForEach(assurer => {
                assurer.Value.Release();
            });
            this.RecycleSelf();
        }

        public void Recycle()
        {
            mLoadedAssurer.Clear();   
        }

        public void Reuse(){}
    }
}
