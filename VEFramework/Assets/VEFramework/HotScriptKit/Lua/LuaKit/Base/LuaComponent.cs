/****************************************************************************
 * Copyright (c) 2018.12 liangxie
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


namespace VEFramework.HotScriptKit 
{
	using UnityEngine;
# if DEFINE_VE_LUA
	using LuaInterface;
	public class LuaComponent : MonoBehaviour
	{
		public static bool isFirstLaunch = true;
		/// <summary>  
		/// 提供给外部手动执行LUA脚本的接口  
		/// </summary>  
		public bool Initilize(string path)
		{
			LuaPath = path;
			Init();
			return true;
		}

		//lua路径，不用填缀名，可以是bundle
		[Tooltip("script path")] public string LuaPath;

		public string LuaFilePath;

		public string LuaClass
		{
			get { return LuaClassName; }
		}
		private string   LuaClassName  = null;


		public LuaTable LuaModule
		{
			get { return mSelfLuaTable; }
		}
		private LuaTable mSelfLuaTable = null;

		//初始化函数，可以被重写，已添加其他
		protected virtual bool Init()
		{		
			mSelfLuaTable = ToLuaManager.Instance.AddLuaFile(LuaPath, gameObject);
			LuaClassName = CallLuaFunctionRString("getClassName");
			
			mSelfLuaTable["gameObject"] = gameObject;
			mSelfLuaTable["transform"] = transform;

			return true;
		}

		private string CallLuaFunctionRString(string name, params object[] args)
		{
			string resault = null;
			if (mSelfLuaTable != null)
			{
				LuaFunction func = mSelfLuaTable.GetLuaFunction(name);
				if (null == func)
				{
					return resault;
				}

				func.BeginPCall();
				func.Push(mSelfLuaTable);
				foreach (var o in args)
				{
					func.Push(o);
				}

				func.PCall();
				resault = func.CheckString();
				func.EndPCall();
			}

			return resault;
		}


		public void CallLuaFunction(string name, params object[] args)
		{
			if (mSelfLuaTable != null)
			{
				LuaFunction func = mSelfLuaTable.GetLuaFunction(name);
				if (null == func)
				{
					return;
				}

				func.BeginPCall();
				func.Push(mSelfLuaTable);
				foreach (var o in args)
				{
					func.Push(o);
				}
				func.PCall();
				func.EndPCall();
			}
		}

		public void LuaDispose()
		{
			if (null != mSelfLuaTable)
			{
				mSelfLuaTable.Dispose();
				mSelfLuaTable = null;
			}
		}

		void Awake()
		{
			if (Initilize(LuaPath))
				CallLuaFunction(ToLuaManager.LuaMonoFunctionName.Awake);
		}

		void OnEnable()
		{
			CallLuaFunction(ToLuaManager.LuaMonoFunctionName.OnEnable);
		}

		void Start()
		{
			CallLuaFunction(ToLuaManager.LuaMonoFunctionName.Start);
		}

		void Update()
		{
			CallLuaFunction(ToLuaManager.LuaMonoFunctionName.Update);
		}

		void OnDisable()
		{
			CallLuaFunction(ToLuaManager.LuaMonoFunctionName.OnDisable);
		}

		void OnDestroy()
		{
			CallLuaFunction(ToLuaManager.LuaMonoFunctionName.OnDestroy);
			LuaDispose();

		}
        public void DisposeLuaTable()
		{
			LuaDispose();
        }
	}
# endif
}