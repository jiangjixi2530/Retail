using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Util.Extend
{
    public static class ObjectExtension
    {
        /// <summary>
        /// 根据对象内容返回字符串
        /// </summary>
        /// <param name="obj">待转换对象</param>
        /// <param name="defaultVal">默认值（可不传）</param>
        /// <returns></returns>
        public static string ObjToString(this object obj, string defaultVal = null)
        {
            try
            {
                if (null != obj)
                {
                    return Convert.ToString(obj);
                }
                return !string.IsNullOrEmpty(defaultVal) ? defaultVal : null;
            }
            catch
            {
                return null;
            }
        }


        public static int ObjToInt(this object obj, int defaultVal = 0)
        {
            try
            {
                if (null == obj)
                {
                    return defaultVal;
                }
                return Convert.ToInt32(obj);
            }
            catch
            {
                return defaultVal;
            }
        }
    }
}
