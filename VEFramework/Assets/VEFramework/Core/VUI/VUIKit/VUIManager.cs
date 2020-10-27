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
	using UnityEngine;
    using System.Collections.Generic;

    public class VUIManager : VEManagers<VUIManager>
    {
		private const string mVrootAssetPath = "VEFramework/Core/VUI/Resources/VRoot";

		private List<IBaseUI> mUIStack;

		private VUIData mDefault_ViewData;
		private VUIData mDefault_WindowData;

		public static VRoot MainRoot;

        public override string ManagerName
        {
            get
            {
                return "VUIManager";
            }
        }

		public override void Init()
		{
			mUIStack = new List<IBaseUI>();
			mDefault_ViewData = new VUIData(true,false);
			mDefault_WindowData = new VUIData(false,true);
			GameObject vroot = null;
			var roottrs = transform.Find("VRoot");
			if(roottrs == null)
			{
				vroot = VAsset.Instance.LoadSync<GameObject>(mVrootAssetPath);
				if(vroot != null)
					vroot.transform.SetParent(transform,false);
			}
			else
			{
				vroot = roottrs.gameObject;
			}
			MainRoot = new VRoot(vroot);
		}


	#region 对外方法
		//打开界面的方法
		public IBaseUI OpenView(string AssestPath,bool bMonoBehaviour = false)
		{
			return Open(AssestPath,mDefault_ViewData,bMonoBehaviour);
		}
		//打开窗口的方法
		public IBaseUI OpenWindow(string AssestPath,bool bMonoBehaviour = false)
		{
			return Open(AssestPath,mDefault_WindowData,bMonoBehaviour);
		}
		
		/// <summary>
        /// 通用打开方法
        /// </summary>
        /// <param name="AssestPath">资产路径</param>
        /// <param name="UIData">UI数据</param>
        /// <param name="bMonoBehaviour">UI是否开启生命周期</param>
		public IBaseUI Open(string AssestPath,IUIData UIData,bool bMonoBehaviour = false)
		{
			IBaseUI UI = CheckCache(AssestPath,UIData);
			if(UI != null)
				return UI;
			UI = CreateUISync(AssestPath);
			if(UI == null)
				return UI;

			Push(UI,UIData);
			UI.Init(AssestPath,UIData,bMonoBehaviour);
			UI.Show();

			return UI;
		}

		public void OpenViewAsync(string AssestPath,Action<IBaseUI> finishCallback,bool bMonoBehaviour = false)
		{
			OpenAsync(AssestPath,finishCallback,mDefault_ViewData,bMonoBehaviour);
		}
		public void OpenWindowAsync(string AssestPath,Action<IBaseUI> finishCallback,bool bMonoBehaviour = false)
		{
			OpenAsync(AssestPath,finishCallback,mDefault_WindowData,bMonoBehaviour);
		}

		public void OpenAsync(string AssestPath,Action<IBaseUI> finishCallback,IUIData UIData,bool bMonoBehaviour = false)
		{
			IBaseUI ui = CheckCache(AssestPath,UIData);
			if(ui != null)
			{
				finishCallback.Invoke(ui);
				return;
			}
			CreateUIAsync(AssestPath,(UI)=>{
				if(UI == null)
					finishCallback.Invoke(null);

				Push(UI,UIData);
				UI.Init(AssestPath,UIData,bMonoBehaviour);
				UI.Show();

				finishCallback.Invoke(UI);
			});
		}

		public void Hide(IBaseUI UI)
		{
			UI.Hide();
		}

		public bool Close(IBaseUI UI)
		{
			var bPop = Pop(UI);
			UI.Close();
			UI.ClearAssest();
			return bPop;
		}
	#endregion
		private void Push(IBaseUI UI,IUIData UIData)
		{
			if(UIData.InQueue)
			{
				if(mUIStack.Count > 0)
					Hide(mUIStack[mUIStack.Count - 1]);
				mUIStack.Add(UI);
			}
		}

		private void RePush(IBaseUI UI,IUIData UIData)
		{
			if(UIData.InQueue)
			{
				if(mUIStack.Count > 0)
					Hide(mUIStack[mUIStack.Count - 1]);
				mUIStack.Add(UI);
			}
		}

		private bool Pop(IBaseUI UI)
		{
			if(!UI.UIData.InQueue)
				return false;
			if(mUIStack.Count == 0 || !(mUIStack[mUIStack.Count - 1] == UI))
				return false;
			mUIStack.Remove(UI);	
			if(mUIStack.Count > 0)
				mUIStack[mUIStack.Count - 1].Show();
			return true;
		}


		private IBaseUI CheckCache(string AssestPath,IUIData UIData)
		{
			if(UIData.InQueue && !UIData.CreateNew)
			{
				for(int i = 0;i<mUIStack.Count;i++)
				{
					if(mUIStack[i].Name.Equals(AssestPath))
					{
						mUIStack.Remove(mUIStack[i]);
						return mUIStack[i];
					}
				}
			}
			return null;
		}
		private IBaseUI CreateUISync(string AssestPath)
		{
			var gameobject = VAsset.Instance.LoadSync<GameObject>(AssestPath);
			if(gameobject == null)
				return null;
			gameobject.transform.SetParent(MainRoot.UIAttach,false);
			var	UI = gameObject.GetComponent<IBaseUI>();
			if(UI == null)
				UI = gameObject.AddComponent<VBaseUI>();
			return UI;
		}

		private void CreateUIAsync(string AssestPath,Action<IBaseUI> finishCallback)
		{
			VAsset.Instance.LoadAsync<GameObject>(AssestPath,(gameobject)=>{
				if(gameobject == null)
				{
					finishCallback.Invoke(null);
					return;
				}
				gameobject.transform.SetParent(MainRoot.UIAttach,false);
				var	UI = gameObject.GetComponent<IBaseUI>();
				if(UI == null)
					UI = gameObject.AddComponent<VBaseUI>();
				finishCallback.Invoke(UI);				
			});
		}
	}
}
