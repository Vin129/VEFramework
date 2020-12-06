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
    public class XLuaManager:Singleton<XLuaManager>
    {
        public const string LuaEnterFile  = "Framework/init.lua";

		private XLuaManager(){}

        public override void InitFinished()
        {
            Init();
        }

        //lua环境
        private static LuaEnv mLuaEnv = null;

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
            mLuaEnv = new LuaEnv();
            mLuaEnv.AddLoader(VAsset.Instance.XLuaLoader);
            var enterFile = LuaEnterFile;
            mLuaEnv.DoString(VAsset.Instance.XLuaLoader(ref enterFile));
            AddMatterFunction(MatterFunctionName.CreateLuaFile);
            AddMatterFunction(MatterFunctionName.MonoUpdate);
        }
        
        private void Update(float deltaTime)
        {
            if (mLuaEnv != null)
            {
                mLuaEnv.Tick();
            }
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
            var func = mLuaEnv.Global.Get<LuaFunction>(funcName);
            if (null == func)
            {
                return false;
            }

            mMatterFunctionMap.Add(funcName, func);
            return true;
        }

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

        //销毁
        public void Destroy()
        {
            //记得释放资源
            foreach (var pair in mMatterFunctionMap)
            {
                pair.Value.Dispose();
            }

            mMatterFunctionMap.Clear();
            mLuaEnv.Dispose();
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

    #endif
        }

        public static void Dispose()
        {
            mInstance = null;
            if (mLuaEnv != null)
            {
                mLuaEnv.Dispose();
                mLuaEnv = null;
            }
        }
    }
#endif
}