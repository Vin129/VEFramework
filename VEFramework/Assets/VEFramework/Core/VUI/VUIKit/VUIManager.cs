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
    using System;

    public class VUIManager : VEManagers<VUIManager>
    {
		private const string mVrootAssetPath = "VEFramework/Core/VUI/Resources/VRoot";
		private Dictionary<string,IBaseUI> mUIOpenList;
		private Dictionary<string,IBaseUI> mUIHideList;

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
			mUIOpenList = new Dictionary<string, IBaseUI>();
			mUIHideList = new Dictionary<string, IBaseUI>();
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
		public IBaseUI Open(string AssestPath,bool bMonoBehaviour = false)
		{
			IBaseUI UI = CheckCache(AssestPath);
			if(UI != null)
				return UI;
			UI = CreateUISync(AssestPath);
			if(UI == null)
				return UI;
			UI.Init(AssestPath,bMonoBehaviour);
			mUIOpenList.Add(AssestPath,UI);
			UI.Show();
			return UI;
		}

		public void OpenAsync(string AssestPath,Action<IBaseUI> finishCallback,bool bMonoBehaviour = false)
		{
			IBaseUI ui = CheckCache(AssestPath);
			if(ui != null)
			{
				finishCallback.Invoke(ui);
				return;
			}
			CreateUIAsync(AssestPath,(UI)=>{
				if(UI == null)
					finishCallback.Invoke(null);
				UI.Init(AssestPath,bMonoBehaviour);
				mUIOpenList.Add(AssestPath,UI);
				UI.Show();
				finishCallback.Invoke(UI);
			});
		}

		public bool Hide(string AssestPath)
		{
			if(mUIOpenList.ContainsKey(AssestPath))
			{
				var UI = mUIOpenList[AssestPath];
				mUIOpenList.Remove(AssestPath);
				mUIHideList.Add(AssestPath,UI);
				UI.Hide();
				return true;
			}	
			return false;
		}

		//TODO 资源清理逻辑
		public bool Close(string AssestPath)
		{
			if(mUIOpenList.ContainsKey(AssestPath))
			{
				var UI = mUIOpenList[AssestPath];
				mUIOpenList.Remove(AssestPath);
				UI.Close();
				return true;
			}
			if(mUIHideList.ContainsKey(AssestPath))
			{
				var UI = mUIHideList[AssestPath];
				mUIHideList.Remove(AssestPath);
				UI.Close();
				return true;
			}
			return false;
		}
	#endregion
		private IBaseUI CheckCache(string AssestPath)
		{
			if(mUIOpenList.ContainsKey(AssestPath))
				return mUIOpenList[AssestPath];
			if(mUIHideList.ContainsKey(AssestPath))
			{
				var UI = mUIHideList[AssestPath];
				mUIHideList.Remove(AssestPath);
				mUIOpenList.Add(AssestPath,UI);
				UI.Show();
				return UI;
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
