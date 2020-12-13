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
    using UnityEngine;
    //XLua&ToLua 虚拟机接口
#if DEFINE_VE_XLUA
    using XLua;
	public interface ILuaEnv       
	{
        LuaEnv LuaEnv {get;}
        LuaTable AddLuaFile(params object[] args);
        LuaComponent AddLuaComponent(GameObject go, string path);
        LuaComponent GetLuaComponent(GameObject go, string path);
    }
#elif DEFINE_VE_TOLUA
    using LuaInterface;
    public interface ILuaEnv 
    {
        LuaState LuaEnv {get;}
        LuaComponent AddLuaComponent(GameObject go, string path);
        LuaComponent GetLuaComponent(GameObject go, string path);
    }
#else
	public interface ILuaEnv {}
#endif
}
