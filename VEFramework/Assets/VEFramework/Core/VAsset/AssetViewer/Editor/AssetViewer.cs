namespace VEFramework
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEditor;
	using EGL = UnityEditor.EditorGUILayout;
	using GL = UnityEngine.GUILayout;
	using System;
	using System.Linq;

	public class AssetViewer : EditorWindow 
    {
        [MenuItem("VETool/VAsset/Viewer")]
		private static void ShowWindow() {
			var window = GetWindow<AssetViewer>();
			window.minSize = new Vector2(700,300);
			window.titleContent = new GUIContent("AssetViewer");
			window.Show();
		}

		private Vector2 scrollRect;
        private void OnGUI() 
        {
            if(!Application.isPlaying)
            {
                EGL.HelpBox("未运行",MessageType.Warning);
                return;
            }
			GUI.skin.label.richText = true;
			scrollRect = GL.BeginScrollView(scrollRect,"box");
			DrawAssurer(ABManager.Instance);
			DrawAssurer(ResManager.Instance);
			DrawAssurer(NetAssetManager.Instance);
			GL.EndScrollView();
        }


		private void DrawAssurer(VAssetManager Manager)
		{
			GL.BeginVertical("OL box");
			GL.Label(Manager.ManagerName);
			var list  = Manager.GetAssurerList();
			if(list.Count  == 0)
			{
				EGL.HelpBox("暂无资产",MessageType.Info);
			}
			else
			{
				list.ForEach(assurer=>
				{
					GL.BeginVertical("GroupBox");
					GL.BeginHorizontal();
					GL.Label(String.Format("{0} : Ref {1}",assurer.Value.AssetPath,assurer.Value.UseCount));
					if(GL.Button("Kill",GUILayout.Width(100)))
					{
						assurer.Value.ForceRecycle();
					}
					GL.EndHorizontal();
					GL.EndVertical();
					GL.Space(2);
				});
			}



			GL.EndVertical();
		}

    }
}
