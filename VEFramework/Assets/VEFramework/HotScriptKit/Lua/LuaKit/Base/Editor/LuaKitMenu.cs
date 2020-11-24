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
    using UnityEditor;
    using VEFramework;
    [InitializeOnLoad]
    public static class LuaKitMenu
    {
        static bool beCheck = false;
        static LuaKitMenu()
        {
            if(!PlayerSettingExtensions.ContainsSymbols(ScriptBaseSetting.LuaDefineSymbol) && !beCheck)
            {
                if (EditorUtility.DisplayDialog("提示", "检测您正在使用HotScriptKit中LuaKit模块,需添加模块宏以确保模块可正常使用", "添加", "取消"))
                {
                    beCheck = true;
                    PlayerSettingExtensions.SetDefineSymbols(ScriptBaseSetting.LuaDefineSymbol);
                    AssetDatabase.Refresh();
                }

                beCheck = false;
            }
        }
    }
}
