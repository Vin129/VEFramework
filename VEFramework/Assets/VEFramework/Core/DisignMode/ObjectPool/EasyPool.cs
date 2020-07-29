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
	using System.Collections.Generic;
	public interface IReusable
	{
		void Reuse();
		void Recycle();
	}

    public class EasyPool<T> : Singleton<EasyPool<T>> where T : IReusable,new()
	{
		private Queue<T> mCache;
		private EasyPool()
		{
			mCache = new Queue<T>();
		}

		public override void InitFinished()
		{
			
		}

		public T Get()
		{
			if(mCache.Count <= 0)
				return new T();
			var v = mCache.Dequeue();
			v.Reuse();
			return v;
		}

		public void Recycle(T t)
		{
			t.Recycle();
			mCache.Enqueue(t);
		}

		public void Release()
		{
			mCache.Clear();
		}
	}


	public static class EasyPoolExtension
	{
		public static void RecycleSelf(this ABAssurer aber)
        {
            EasyPool<ABAssurer>.Instance.Recycle(aber);
        }
		public static void RecycleSelf(this ABPathAnalysis analysis)
        {
            EasyPool<ABPathAnalysis>.Instance.Recycle(analysis);
        }
	}
}
