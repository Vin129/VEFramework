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
namespace VEFramework
{
	using System.IO;
    using System.Collections.Generic;
    using UnityEngine;

    public class PathUtil
	{
        public static void SaveExternalAsset(string url,byte[] bytes)
        {
            CreateDirectory(AssetCustomSetting.ExternalAssetDir);
            var AssetPath = AssetCustomSetting.ExternalAssetDir + url.GetHashCode();
            File.WriteAllBytes(AssetPath,bytes);
            Log.I("SaveAsset:{0}",url);
        }

        public static bool ExternalAssetExist(string url,ref string AssetPath)
        {
            CreateDirectory(AssetCustomSetting.ExternalAssetDir);
            AssetPath = AssetCustomSetting.ExternalAssetDir + url.GetHashCode();
            return IsPersistentFileExists(AssetPath);
        }


        ///<summary>
        ///contactPath + fileNameOrPath
        ///</summary>
		public static List<string> GetAllFiles(string rootPath,SearchOption searchOpt = SearchOption.AllDirectories)
		{
			List<string> strDirs ;
			if(SearchOption.TopDirectoryOnly == searchOpt)
			{
				strDirs = new List<string>();
				strDirs.Add(rootPath);
			}
			else
				strDirs = PathUtil.GetAllPath(rootPath,searchOpt);
			List<string> strFiles = new List<string>();
			foreach(var dir in strDirs)
			{
                if(false == IsDirectory(dir) && IsFileExists(dir) && !dir.EndsWith(".meta"))
                {
                        string filePath = dir.Replace("\\","/");
                        strFiles.Add(filePath);
                }
				else
                {
                    string[] kFiles = Directory.GetFiles(dir, "*.*");
                    foreach(var file in kFiles)
                    {
                        if(file.EndsWith(".meta"))
                            continue;
                        string filePath = file.Replace("\\","/");
                        strFiles.Add(filePath);
                    }
                }
			}
			return strFiles;
		}
    
    public static List<string> GetAllPath(string rootPath,SearchOption searchOpt = SearchOption.AllDirectories)
    {
        List<string> strDirs = new List<string>();
        strDirs.Add(rootPath);
        if(SearchOption.TopDirectoryOnly == searchOpt)
            return strDirs;
        
        string[] dirs = SaveGetDirectories(rootPath,"*.*",searchOpt);
        if(dirs != null)
        {
            foreach(var dir in dirs)
            {
                string strDir = dir.Replace("\\","/");
                strDirs.Add(strDir);
            }
        }
        return strDirs;
    }

    public static void GetPathAndFile(string fileNameOrPath,ref string filePath ,ref string fileName,bool bContainSlash = false)
    {
        fileNameOrPath = fileNameOrPath.Replace('\\', '/');
        int iIdx = fileNameOrPath.LastIndexOf("/");
        if (-1 == iIdx)
        {
            filePath = "";
            fileName = fileNameOrPath;
        }
        else
        {
            if(bContainSlash)
                filePath = fileNameOrPath.Substring(0, iIdx+1);                    
            else
                filePath = fileNameOrPath.Substring(0, iIdx);
            fileName = fileNameOrPath.Substring(iIdx + 1);
        }
    }

    public static string GetPathUnderAssets(string strIn,bool bContainSlash = true)
    {
        strIn = strIn.Replace('\\','/');
        strIn = strIn.Replace(Application.dataPath,"Assets");
        if(true == bContainSlash)
            return strIn + "/";
        return strIn;
    }

    public static List<string> GetFileNameOnly(string strPath,string strContactPath = "",SearchOption kSearchOpt = SearchOption.TopDirectoryOnly)
    {
        return GetFileName(strPath,strContactPath,false,kSearchOpt);
    }

    public static List<string> GetFileNameWithPostfix(string strPath,string strContactPath = "",SearchOption kSearchOpt = SearchOption.TopDirectoryOnly)
    {
        return GetFileName(strPath,strContactPath,true,kSearchOpt);
    }

    public static List<string> GetFullPathName(string strPath,bool bPostfixed,SearchOption kSearchOpt = SearchOption.TopDirectoryOnly)
    {
        List<string> strFiles = new List<string>();
        string[] files = Directory.GetFiles(strPath,"*.*",kSearchOpt);
        foreach(var file in files)
        {
            if(file.EndsWith(".meta"))
                continue;
            string strFile = file.Replace("\\","/");
            if(false == bPostfixed)
            {
                int iIdx = strFile.LastIndexOf('.');
                if(-1 != iIdx)
                    strFile = strFile.Substring(0,iIdx);
            }
            strFiles.Add(strFile);
        }
        return strFiles;
    }

    protected static List<string> GetFileName(string strPath,string strContactPath,bool bPostfixed,SearchOption kSearchOpt)
    {
        List<string> strFiles = new List<string>();
        string[] files = Directory.GetFiles(strPath,"*.*",kSearchOpt);
        foreach(var file in files)
        {
            if(file.EndsWith(".meta"))
                continue;
            string strFile = file.Replace("\\","/");
            int iIdx = strFile.LastIndexOf("/");
            if(-1 != iIdx)
                strFile = strFile.Substring(iIdx+1);
            if(false == bPostfixed)
            {
                iIdx = strFile.LastIndexOf('.');
                if(-1 != iIdx)
                    strFile = strFile.Substring(0,iIdx);
            }
            strFiles.Add(strContactPath + strFile);
        }
        return strFiles;
    }

    public static string[] SaveGetDirectories(string path, string searchPattern, SearchOption searchOption)
    {
        if(IsDirectory(path))
        {
            return Directory.GetDirectories(path,searchPattern,searchOption);
        }
        return null;
    }

    public static bool IsDirectory(string strPath)
    {
        return Directory.Exists(strPath);
    }

    public static bool IsFileExists(string strFullPath)
    {
        return File.Exists(strFullPath);
    }

    public static bool IsPersistentFileExists(string strFullPath)
    {
        return File.Exists(strFullPath);
    }

    public static bool IsStreamingFileExists(string strFullPath)
    {
        return File.Exists(strFullPath);
    }

    public static void CreateDirectory(string strPath)
    {
        if(false == Directory.Exists(strPath))
            Directory.CreateDirectory(strPath);
    }
    
    public static string Res2ABPathConvert(string strPath)
    {
        string strABDir = AssetCustomSetting.AssetBundleDir;
        if(strABDir.EndsWith("/"))
            strABDir.Substring(0,strABDir.Length-1);
        strABDir = strABDir.Replace('\\','/');
        int iIdx = strABDir.LastIndexOf("/");
        strABDir = strABDir.Substring(iIdx+1);
        strPath = strPath.Replace("Resources",strABDir);
        return strPath;
    }

#if UNITY_ANDROID && !UNITY_EDITOR
    protected static AndroidJavaObject m_AndroidJavaObject = null;
#endif
	}
}
