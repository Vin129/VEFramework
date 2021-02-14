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
namespace VEFramework.HotScriptKit  
{
    using System;
    using UnityEngine;
    using System.Collections.Generic;
    #if UNITY_EDITOR	
        using UnityEditor;
    #endif
#if DEFINE_VE_XLUA   
    using XLua;
    using System.IO;
    public class XLuaManager:VLuaManager<XLuaManager>,ILuaEnv
    {
		private XLuaManager(){}
        //lua环境
        private LuaEnv mLuaEnv = null;
        //预存函数提高效率
        protected Dictionary<string, LuaFunction> mMatterFunctionMap;

        public LuaEnv LuaEnv 
        {
            get
            {
                return mLuaEnv;
            }
        }

        //初始化
        public override void Init()
        {
            mSearchPaths = new List<string>();
            mMatterFunctionMap = new Dictionary<string, LuaFunction>();
            AddEditorSearchPath(ScriptBaseSetting.LuaGamePath);

            mLuaEnv = new LuaEnv();
            mLuaEnv.AddLoader(XLuaLoader);
            var enterFile = LuaEnterFile;
            mLuaEnv.DoString(XLuaLoader(ref enterFile));
            AddMatterFunction(MatterFunctionName.CreateLuaFile);
            AddMatterFunction(MatterFunctionName.MonoUpdate);
        }
        public override ILuaEnv GetEnv(){ return Instance;}
        
        public override void Update(float deltaTime)
        {
            if (mLuaEnv != null)
            {
                mLuaEnv.Tick();
            }
            if(mMatterFunctionMap[MatterFunctionName.MonoUpdate] != null)
                mMatterFunctionMap[MatterFunctionName.MonoUpdate].Call(deltaTime);
        }   


        //保存函数
        public override bool AddMatterFunction(string funcName)
        {
            var func = mLuaEnv.Global.Get<LuaFunction>(funcName);
            if (null == func)
            {
                return false;
            }

            mMatterFunctionMap.Add(funcName, func);
            return true;
        }

        //销毁
        public override void Destroy()
        {
            //记得释放资源
            foreach (var pair in mMatterFunctionMap)
            {
                pair.Value.Dispose();
            }

            mMatterFunctionMap.Clear();
            if(mLuaEnv != null)
            {
                mLuaEnv.Dispose();
                mLuaEnv = null;
            }
        }

	#region XLua加载
        public void AddEditorSearchPath(string path)
        { 
        #if UNITY_EDITOR
            if (!Path.IsPathRooted(path))
            {
                Log.E(path + " is not a full path");
                return;
            }
            var fullpath = path;
            fullpath = fullpath.Replace('\\', '/');
            if (fullpath.Length > 0 && fullpath[fullpath.Length - 1] != '/')
            {
                fullpath += '/';
            }
            fullpath += "?.lua";
            mSearchPaths.Add(fullpath);
        #endif
        }

		public byte[] XLuaLoader(ref string fileName)
        {	
        #if XLUA_VASSET || !UNITY_EDITOR
			if(!fileName.EndsWith(".lua"))
				fileName += ".lua";
			return VAsset.Instance.LoadScriptFile(fileName);
        #elif UNITY_EDITOR
            string filePath = string.Empty;
            var name = fileName.Replace(".lua","");
            mSearchPaths.ForEach(value=>{
                if(File.Exists(value.Replace("?",name)))
                {
                    filePath = value.Replace("?",name);
                    return;
                }   
            });
            if(filePath.IsEmptyOrNull())
                return null;
            return File.ReadAllBytes(filePath);
        #endif
        }
	#endregion

    #region LuaEnv通用组件
        public LuaTable AddLuaFile(params object[] args)
        {
            LuaFunction func = mLuaEnv.Global.Get<LuaFunction>(MatterFunctionName.CreateLuaFile);
            if (null == func)
            {
                return null;
            }
            var popValues = func.Call(args);
            LuaTable table = popValues[0] as LuaTable;
            func.Dispose();
            return table;
        }

        public LuaComponent AddLuaComponent(GameObject go, string path)
        {
            LuaComponent luaComp = go.AddComponent<LuaComponent>();
            luaComp.Initilize(path); // 手动调用脚本运行，以取得LuaTable返回值  
            return luaComp;
        }
        public LuaComponent GetLuaComponent(GameObject go, string LuaClassName)
        {
            LuaComponent[] comps = go.GetComponents<LuaComponent>();

            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].LuaClass == LuaClassName)
                    return comps[i];
            }
            return null;
        }
    #endregion
    }
#endif
}