using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Retail.Util
{
    [Serializable]
    public class RetailResult<T>
    {
        /// <summary>
        /// 接口执行是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 接口执行结果
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 接口返回信息
        /// </summary>
        public T Data { get; set; }
        /// <summary>
        /// 扩展
        /// </summary>
        public object Extend { get; set; }

        public static RetailResult<T> ToSuccess(T t, string msg = "成功")
        {
            return new RetailResult<T>() { IsSuccess = true, Data = t, Msg = msg };
        }
        public static RetailResult<T> ToFail(string msg = "", object extend = null)
        {
            return new RetailResult<T>() { IsSuccess = false, Msg = msg, Extend = extend };
        }
    }
}
