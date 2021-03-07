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
#if DEFINE_VE_TOLUA
    using  LuaInterface;
#elif DEFINE_VE_XLUA
    using XLua;
#endif
    public static class LuaPerformer
    {
        #if DEFINE_VE_TOLUA || DEFINE_VE_XLUA
            public static void Call(object luafunction,params object[] args)
            {
                LuaFunction f = (LuaFunction)luafunction;
                f.Call(args);
            }
            public static string RSCall(object luafunction,params object[] args)
            {
                LuaFunction f = (LuaFunction)luafunction;
#if DEFINE_VE_XLUA
            var resultParams = f.Call(args);
                if(resultParams.Length == 0)
                    return null;
                return resultParams[0] as string;
#else
            var result = f.Invoke<string>();
            return result;
#endif
        }
            public static LuaFunction GetFunction(LuaTable mTable,string fName)
            {
#if DEFINE_VE_TOLUA
                return mTable.GetLuaFunction(fName);
#else
                return mTable.Get<LuaFunction>(fName);
#endif
            }
#else
            public static void Call(object luafunction,params object[] args){}
            public static string RSCall(object luafunction,params object[] args){return null;}
        #endif
    }
}

