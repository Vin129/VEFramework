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

	private Assurer mAssurer;
	private void Awake() 
	{
		//初始化VEManager
		gameObject.AddComponent<VEManager>();

		// ResManager.Instance.LoadAsync<GameObject>("Prefabs/Test2/TestView2",(obj)=>{
		// 	if(obj == null)
		// 		return;
		// 	var Obj2 = GameObject.Instantiate(obj);
		// 	Obj2.transform.parent = Node.transform;
		// });

		// ABManager.Instance.LoadAsync<GameObject>("Prefabs/Test1/TestView1",(obj)=>{
		// 	if(obj == null)
		// 		return;
		// 	var Obj2 = GameObject.Instantiate(obj);
		// 	Obj2.transform.parent = Node.transform;
		// });

		// mAssurer = VAsset.Instance.GetAssurerAsync("Scenes/Ground_Night");
		// mAssurer.LoadFinishCallback += (assurer)=>{
		// 	Log.E("Finish");
		// 	// var obj = assurer.Get<GameObject>();
		// 	// if(obj == null)
		// 	// 	return;
		// 	// var Obj2 = GameObject.Instantiate(obj);
		// 	// Obj2.transform.parent = Node.transform;
		// };
		var url = "https://www.baidu.com/img/PCtm_d9c8750bed0b3c7d089fa7d55720d6cf.png";
		// VAsset.Instance.DownloadAsset<Texture2D>(url,(Image)=>{
		// 	if(Image == null)
		// 		return;
		// 	Img1.sprite = Sprite.Create(Image,new Rect(0,0,Image.width,Image.height),Img1.rectTransform.pivot);
		// },bSave:true,bLocalFirst:true);


		// VAsset.Instance.DownloadAsset(url,(bytes)=>{
		// 	if(bytes == null)
		// 		return;
		// 	var Image = new Texture2D(500,400);	
		// 	Image.LoadImage(bytes);
		// 	Img1.sprite = Sprite.Create(Image,new Rect(0,0,Image.width,Image.height),Img1.rectTransform.pivot);
		// },bSave:true,bLocalFirst:true);

		VAsset.Instance.LoadAsync<GameObject>("Prefabs/Test1/TestView1",(obj)=>{
			if(obj == null)
				return;
			var Obj2 = GameObject.Instantiate(obj);
			Obj2.transform.parent = Node.transform;
		});


	}

	private void Update() {

	}
}
