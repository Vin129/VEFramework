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
	///<summary>
	///资源释放模式
	///I_DONT_CARE:I dont care 模式,由VE代管资源释放（延迟释放策略），使用延迟释放策略但不会释放内存中的资源
	///BEGIN_AND_END: Begin and End 模式，有始有终，资源释放完全取决于你
	///</summary>
	public enum AssetUnLoadModeType
	{
		I_DONT_CARE = 1,
		BEGIN_AND_END = 2,
	}

	///<summary>
	/// AssetCustomSetting 基本原则：
	///	Resource文件夹下文件的加载可以使用相对路径 
	/// AssetBundle文件命名分为：1. 路径 + 文件名  2. 文件路径
	///</summary>
	public static class AssetCustomSetting
	{
		public static readonly float AssetKeepTime = 3f;
		public static readonly AssetUnLoadModeType AssetUnLoadMode = AssetUnLoadModeType.I_DONT_CARE;
		public static readonly string ResourceDir = UnityEngine.Application.dataPath + "/Resources/";
		public static readonly string AssetBundlerRuleAssetPath = UnityEngine.Application.dataPath + "/VEFramework/Core/VAsset/AssetRule/AssetBundleRules.asset";

		///<summary>
		///Empty：默认StreamingAssets为AssetBundle文件根目录
		///若需要指定文件夹为AB文件根目录请修改此值在StreamingAssets下创建专属文件夹。 
		///</summary>
		public static string ABManifestFileName
		{
			get
			{
				return "TestAB";
				// return string.Empty;
			}
		}
		public static string ABPostfix
		{
			get
			{
				return ".unity3d";
			}
		}
		public static string PersistentABDir
		{
			get
			{
				if(ABManifestFileName.IsEmptyOrNull())
					return UnityEngine.Application.persistentDataPath + "/";
				return UnityEngine.Application.persistentDataPath + "/" + ABManifestFileName + "/";
			}
		}

		public static string AssetBundleDir
		{
			get
			{
				if(ABManifestFileName.IsEmptyOrNull())
					return UnityEngine.Application.streamingAssetsPath + "/";
				return UnityEngine.Application.streamingAssetsPath + "/" + ABManifestFileName + "/";
			}
		}
	}
}
