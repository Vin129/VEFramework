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
    using LuaInterface;
    #if UNITY_EDITOR	
    using UnityEditor;
    #endif
    public class ToLuaManager:Singleton<ToLuaManager>
    {
        public const string LuaEnterFile  = "Framework/init.lua";

		private ToLuaManager(){}

        public override void InitFinished()
        {
            Init();
        }

        //lua环境
        private static LuaState mLuaState = null;

        //函数名字定义
        public static class MatterFunctionName
        {
            public static readonly string CreateLuaFile = "CreateLuaFile";

            public static readonly string MonoUpdate = "MonoUpdate";
        };

        public static class LuaMonoFunctionName
        {
            public static readonly string Awake         = "Awake";
            public static readonly string OnEnable      = "OnEnable";
            public static readonly string Start         = "Start";
            public static readonly string Update        = "Update";
            public static readonly string OnDisable     = "OnDisable";
            public static readonly string OnDestroy     = "OnDestroy";
        }
        //预存函数提高效率
        protected Dictionary<string, LuaFunction> mMatterFunctionMap = new Dictionary<string, LuaFunction>();

        //初始化
        public void Init()
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
        
        private void Update(float deltaTime)
        {
            if(mMatterFunctionMap[MatterFunctionName.MonoUpdate] != null)
                mMatterFunctionMap[MatterFunctionName.MonoUpdate].Call(deltaTime);
        }   


        public void BindMonoUpdate(ref Action<float> act)
        {
            act = Update;
        }

        //保存函数
        public bool AddMatterFunction(string funcName)
        {
            var func = mLuaState.GetFunction(funcName);
            if (null == func)
            {
                return false;
            }

            mMatterFunctionMap.Add(funcName, func);
            return true;
        }

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

        //销毁
        public void Destroy()
        {
            //记得释放资源
            foreach (var pair in mMatterFunctionMap)
            {
                pair.Value.Dispose();
            }

            mMatterFunctionMap.Clear();
            mLuaState.Dispose();
        }


        public static LuaComponent AddLuaComponent(GameObject go, string path)
        {
            LuaComponent luaComp = go.AddComponent<LuaComponent>();
            luaComp.Initilize(path); // 手动调用脚本运行，以取得LuaTable返回值  
            return luaComp;
        }
        public static LuaComponent GetLuaComponent(GameObject go, string LuaClassName)
        {
            LuaComponent[] comps = go.GetComponents<LuaComponent>();

            for (int i = 0; i < comps.Length; i++)
            {
                if (comps[i].LuaClass == LuaClassName)
                    return comps[i];
            }

            return null;
        }

        public static void AddEditorSearchPath(string path)
        {
    #if UNITY_EDITOR
            if (mLuaState != null)
            {
                mLuaState.AddSearchPath(Application.dataPath + "/" + path);
            }
    #endif
        }

        public static void Dispose()
        {
            mInstance = null;
            if (mLuaState != null)
            {
                mLuaState.Dispose();
                mLuaState = null;
            }
        }
    }
}