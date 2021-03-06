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
# if DEFINE_VE_TOLUA
	using LuaInterface;
#elif DEFINE_VE_XLUA
	using XLua;
#endif

#if DEFINE_VE_TOLUA || DEFINE_VE_XLUA
	public class LuaComponent : MonoBehaviour
	{
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
		[Tooltip("script path")] 
		public string LuaPath;
		public string LuaFilePath;
		public string LuaClass
		{
			get { return mLuaClassName; }
		}
		private string mLuaClassName  = null;

		public ILuaEnv LuaEnv
		{
			get
			{
			# if DEFINE_VE_TOLUA
				return ToLuaManager.Instance;
			#elif DEFINE_VE_XLUA
				return XLuaManager.Instance;
			#endif 
			}
		}
		public LuaTable LuaModule
		{
			get { return mSelfLuaTable; }
		}
		private LuaTable mSelfLuaTable = null;

		//初始化函数，可以被重写，已添加其他
		protected virtual bool Init()
		{		
			mSelfLuaTable = LuaEnv.AddLuaFile(LuaPath, gameObject);
			mLuaClassName = CallLuaFunctionRString("getClassName");
			mSelfLuaTable["gameObject"] = gameObject;
			mSelfLuaTable["transform"] = transform;
			return true;
		}

		private string CallLuaFunctionRString(string name, params object[] args)
		{
			if(mSelfLuaTable == null)
				return null;
			var result = string.Empty;
			var function = LuaPerformer.GetFunction(mSelfLuaTable,name);
			result = LuaPerformer.RSCall(function);
			function.Dispose();
			return result;
		}


		public void CallLuaFunction(string name, params object[] args)
		{
			if(mSelfLuaTable == null)
				return;
			var function = LuaPerformer.GetFunction(mSelfLuaTable,name);
			LuaPerformer.Call(function);
			function.Dispose();
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
				CallLuaFunction(LuaMonoFunctionName.Awake);
		}

		void OnEnable()
		{
			CallLuaFunction(LuaMonoFunctionName.OnEnable);
		}

		void Start()
		{
			CallLuaFunction(LuaMonoFunctionName.Start);
		}

		void Update()
		{
			CallLuaFunction(LuaMonoFunctionName.Update);
		}

		void OnDisable()
		{
			CallLuaFunction(LuaMonoFunctionName.OnDisable);
		}

		void OnDestroy()
		{
			CallLuaFunction(LuaMonoFunctionName.OnDestroy);
			LuaDispose();

		}
        public void DisposeLuaTable()
		{
			LuaDispose();
        }
	}
# endif
}