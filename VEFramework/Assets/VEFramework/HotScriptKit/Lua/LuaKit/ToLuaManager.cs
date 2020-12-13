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
#if DEFINE_VE_TOLUA   
    using LuaInterface;
    public class ToLuaManager:VLuaManager<ToLuaManager>,ILuaEnv
    {
		private ToLuaManager(){}

        public override void InitFinished()
        {
            Init();
        }

        //lua环境
        private static LuaState mLuaState = null;

        //预存函数提高效率
        protected Dictionary<string, LuaFunction> mMatterFunctionMap = new Dictionary<string, LuaFunction>();

        public LuaState LuaEnv 
        {
            get
            {
                return mLuaState;
            }
        }
        //初始化
        public override void Init()
        {
    #if UNITY_EDITOR
            LuaFileUtils.Instance.beZip = AppSetting.AssetBundleOpen;
            // LuaFileUtils.Instance.beZip = false;
    #else
            LuaFileUtils.Instance.beZip = AppSetting.AssetBundleOpen;
    #endif
            mLuaState = new LuaState();
            mLuaState.LogGC = false;
            libLoader();
            mLuaState.Start();
            mLuaState.DoFile(LuaFileUtils.Instance.beZip ? LuaEnterFile : LuaEnterFile);
            AddMatterFunction(MatterFunctionName.CreateLuaFile);
            AddMatterFunction(MatterFunctionName.MonoUpdate);
        }

        public override ILuaEnv GetEnv(){ return Instance;}

        private void libLoader()
        {
            mLuaState.OpenLibs(LuaDLL.luaopen_pb);
            mLuaState.OpenLibs(LuaDLL.luaopen_lpeg);
            mLuaState.OpenLibs(LuaDLL.luaopen_bit);
            mLuaState.OpenLibs(LuaDLL.luaopen_socket_core);
            mLuaState.LuaSetTop(0);
            bind();  
        }

        private void bind()
        {
            LuaBinder.Bind(mLuaState);
            DelegateFactory.Init();
        }
        
        public override void Update(float deltaTime)
        {
            if(mMatterFunctionMap[MatterFunctionName.MonoUpdate] != null)
                mMatterFunctionMap[MatterFunctionName.MonoUpdate].Call(deltaTime);
        }   

        //保存函数
        public override bool AddMatterFunction(string funcName)
        {
            var func = mLuaState.GetFunction(funcName);
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
            if(mLuaState != null)
            {
                mLuaState.Dispose();
                mLuaState = null;
            }
        }

    #region LuaEnv通用组件
        public LuaTable AddLuaFile(params object[] args)
        {
            LuaFunction func = mLuaState.GetFunction(MatterFunctionName.CreateLuaFile);
            if (null == func)
            {
                return null;
            }

            func.BeginPCall();
            foreach (var o in args)
            {
                func.Push(o);
            }

            func.PCall();
            LuaTable table = func.CheckLuaTable();
            func.EndPCall();
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
        public static void AddEditorSearchPath(string path)
        {
    #if UNITY_EDITOR
            if (mLuaState != null)
            {
                mLuaState.AddSearchPath(Application.dataPath + "/" + path);
            }
    #endif
        }
    }
#endif
}