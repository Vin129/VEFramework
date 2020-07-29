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
using UnityEngine;
using UnityEngine.UI;
using VEFramework;

public class GamePipeline : MonoBehaviour {
	public GameObject Node;
	public Image Img1;
	public Image Img2;
	private void Awake() 
	{
		//初始化VEManager
		gameObject.AddComponent<VEManager>();
		// "Texture/UI/JumpIcon/tz_icon_02"
		Img1.sprite = ABManager.Instance.LoadSync<Sprite>("Image/UI/Formation/kp_h_03");
		Img2.sprite = ABManager.Instance.LoadSync<Sprite>("Develop/TextureSta/UI/HeadIcon/txk_icon_01");

		var Obj1 = GameObject.Instantiate(ABManager.Instance.LoadSync<GameObject>("Prefabs/UI/PlayerDetail/PlayerDetailView"));
		Obj1.transform.parent = Node.transform;

		ABManager.Instance.LoadAsync<GameObject>("Prefabs/UI/MatchEngine/MainView/MatchEngineView",(obj)=>{
			if(obj == null)
				return;
			var Obj2 = GameObject.Instantiate(obj);
			Obj2.transform.parent = Node.transform;
		});

	}
}
