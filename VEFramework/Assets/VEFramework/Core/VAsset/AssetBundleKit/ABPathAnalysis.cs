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
    public class ABPathAnalysis:IReusable
    {
        public static ABPathAnalysis EasyGet()
		{
			var analysis = EasyPool<ABPathAnalysis>.Instance.Get();
			return analysis;
		}

        public bool B_FileExist;
        public bool B_UnloadTag;
        public string AssetPath;
        public string FileName;
        public string RealPath;
        public ABPathAnalysis(){}
        public void Analyze(string AssetPath,bool bPostfix,bool bUnloadTag)
        {
            this.AssetPath = AssetPath;
            this.B_UnloadTag = bUnloadTag;
            this.RealPath = ABManager.Instance.GetAssetbundleRealPath(AssetPath,ref B_FileExist,ref FileName,bPostfix);
        }
        public void Reuse(){}
        public void Recycle()
        {
            B_FileExist = false;
            AssetPath = null;
            FileName = null;
            RealPath = null;
        }
    }
}
