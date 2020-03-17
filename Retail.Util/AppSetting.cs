using System;
using System.Collections.Generic;
using System.Text;

namespace Retail.Util
{
    public class AppSetting
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public static string DbConnectionString { get; set; }

        /// <summary>
        /// 当前时间
        /// </summary>
        public static DateTime TimeNow { get => DateTime.Now; }
    }
}
