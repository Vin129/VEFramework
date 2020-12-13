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
    public class VLuaManager<T> : VLuaManager where T : VLuaManager,ILuaEnv
    {   
        protected static T mInstance;
		public static T Instance 
		{
			get
			{
				if(mInstance == null)
				{
					mInstance = SingletonCreator.CreateSingleton<T>();
				}
				return mInstance; 
			}
		}
    }

    public abstract class VLuaManager:ISingleton
    {
        protected VLuaManager(){}
        public const string LuaEnterFile  = "Framework/init.lua";
        //函数名字定义
        protected List<string> mSearchPaths;
        public virtual ILuaEnv GetEnv(){ return null ;}
        public virtual void InitFinished(){ Init(); }
        public virtual void Init(){}
        public virtual void Update(float deltaTime){}
        public virtual void BindMonoUpdate(ref Action<float> act){act = Update;}
        public virtual bool AddMatterFunction(string funcName){return false;}
        public virtual void Destroy(){}
    }

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
}

