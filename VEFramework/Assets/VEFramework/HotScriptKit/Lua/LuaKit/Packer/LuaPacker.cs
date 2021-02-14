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
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    public static class LuaPacker
    {
        public static void CopyLuaFilesToResource()
        {
            ClearAllLuaFiles();
            string destDir = Application.dataPath + "/Resources/Lua";
            CopyLuaBytesFiles(ScriptBaseSetting.LuaGamePath, destDir);
        }

        public static void ClearAllLuaFiles()
        {
            string destDir = Application.dataPath + "/Resources/Lua";
            if (Directory.Exists(destDir))
            {
                Directory.Delete(destDir, true);
            }
        }

        static void CopyLuaBytesFiles(string sourceDir, string destDir, bool appendext = true, string searchPattern = "*.lua", SearchOption option = SearchOption.AllDirectories)
        {
            if (!Directory.Exists(sourceDir))
            {
                return;
            }
            string[] files = Directory.GetFiles(sourceDir, searchPattern, option);
            int len = sourceDir.Length;
            if (sourceDir[len - 1] == '/' || sourceDir[len - 1] == '\\')
            {
                --len;
            }         
            for (int i = 0; i < files.Length; i++)
            {
                string str = files[i].Remove(0, len);
                string dest = destDir + "/" + str;
                if (appendext) dest += ".bytes";
                string dir = Path.GetDirectoryName(dest);
                Directory.CreateDirectory(dir);
                File.Copy(files[i], dest, true);
            }
        }
    }
}

