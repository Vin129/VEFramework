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
	using System;
    using System.Linq;
    using UnityEngine;

    public static class Log {
		#region Base		
		public static void I(object message)
		{
			UnityEngine.Debug.Log(message.ToString());
		}
		public static void W(object message)
		{
			UnityEngine.Debug.LogWarning(message.ToString());
		}

		public static void E(object message)
		{
			UnityEngine.Debug.LogError(message.ToString());
		}
		public static void I(object message, params object[] args)
		{
			UnityEngine.Debug.LogFormat(message.ToString(),args);
		}
		public static void IColor(object message,string color,params object[] args)
		{
			UnityEngine.Debug.LogFormat("<color=#{0}>{1}</color>",color,string.Format(message.ToString(),args));
		}
		public static void W(object message, params object[] args)
		{
			UnityEngine.Debug.LogWarningFormat(message.ToString(),args);
		}

		public static void E(object message, params object[] args)
		{
			UnityEngine.Debug.LogErrorFormat(message.ToString(),args);
		}
		# endregion


		#region  Extension
		public static void ErrorReport(this Exception e)
		{
			if(e == null)
				return;
			var message = e.Message + "\n" + e.StackTrace.Split('\n').FirstOrDefault();
			I(message);
		}

		public static void LogInfo<T>(this T obj,params object[] args)
		{
			UnityEngine.Debug.LogFormat(obj.ToString(),args);
		}

		#endregion
	}
}
