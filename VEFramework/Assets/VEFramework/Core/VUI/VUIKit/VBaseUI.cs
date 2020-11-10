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
    public class VBaseUI : MonoBehaviour,IBaseUI
    {	
        protected List<string> mAssetPaths;
        protected string mName;
        protected IUIData mUIData;
        protected IAssurerLoader mLoader;
        protected bool bMonoBehaviour = false;   

		/// <summary>
        /// 资源深度释放
        /// </summary>
        protected bool bClearAsset = true;
        public string Name { get{return mName;} }
        public IUIData UIData { get{return mUIData;} }

        public IAssurerLoader Loader { get{return mLoader;} }

        public virtual void Init(string Name,IUIData UIData,IAssurerLoader loader,bool bMonoBehaviour = false)
        { 
            mName = Name;
            mUIData = UIData;
            mLoader = loader;
            this.bMonoBehaviour = bMonoBehaviour;
            mAssetPaths = new List<string>();
            OnInit();
        }

        public virtual void OnInit()
        {

        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
           gameObject.SetActive(false);
        }

        public virtual void CloseSelf()
        {
            VUIManager.Instance.Close(this);
        }

        public virtual void Close()
        {
            VUIManager.Instance.Close(this);
        }

        public virtual void ClearAssest()
        {
            GameObject.Destroy(gameObject);
            Loader.Release(bClearAsset);
        }

        protected virtual void Destroy()
        {

        }

    #region MonoBehaviour
        private void ExecuteBehaviour(ExecuteBehaviourType Type)
        {
            if(!bMonoBehaviour)
                return;
            switch(Type)
            {
                case ExecuteBehaviourType.Awake:

                break;
                case ExecuteBehaviourType.OnEnable:

                break;
                case ExecuteBehaviourType.Start:

                break;
                case ExecuteBehaviourType.Update:

                break;
                case ExecuteBehaviourType.OnDisable:

                break;
                case ExecuteBehaviourType.OnDestroy:
                    Destroy();
                break;
            }
        }

        protected virtual void Awake() 
		{
            ExecuteBehaviour(ExecuteBehaviourType.Awake);
		}

        protected virtual void OnEnable() 
        {
            ExecuteBehaviour(ExecuteBehaviourType.OnEnable);
        }

		protected virtual void Start() 
		{
            ExecuteBehaviour(ExecuteBehaviourType.Start);			
		}

        protected virtual void Update() 
        {
            ExecuteBehaviour(ExecuteBehaviourType.Update);           
        }

        protected virtual void OnDisable() 
        {
            ExecuteBehaviour(ExecuteBehaviourType.OnDisable);            
        }

        protected virtual void OnDestroy() 
        {
            ExecuteBehaviour(ExecuteBehaviourType.OnDestroy);            
        }

        #endregion
    }
}

