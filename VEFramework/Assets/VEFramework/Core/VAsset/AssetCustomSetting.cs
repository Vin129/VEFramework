﻿/****************************************************************************
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
	/// AssetCustomSetting 基本原则：
	///	Resource文件夹下文件的加载可以使用相对路径 
	/// AssetBundle文件命名分为：1. 路径 + 文件名  2. 文件路径
	///</summary>
	public static class AssetCustomSetting
	{

		public static string ABPostfix
		{
			get
			{
				return ".unity3d";
			}
		}
		public static string PersistentDir
		{
			get
			{
				return UnityEngine.Application.persistentDataPath;
			}
		}

		public static string AssetBundleDir
		{
			get
			{
				return UnityEngine.Application.streamingAssetsPath;
			}
		}
	}
}
