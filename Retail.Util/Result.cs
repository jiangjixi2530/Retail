using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Retail.Util
{
    [Serializable]
    public class Result<T>
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

        public static Result<T> Success(T t, string msg = "成功")
        {
            return new Result<T>() { IsSuccess = true, Data = t, Msg = msg };
        }
        public static Result<T> Fail(string msg = "", object extend = null)
        {
            return new Result<T>() { IsSuccess = false, Msg = msg, Extend = extend };
        }
    }
}
