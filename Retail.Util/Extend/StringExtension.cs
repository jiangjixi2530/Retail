using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Retail.Util.Extend
{
    /// <summary>
    /// 字符扩展类
    /// </summary>
    public static class StringExtension
    {
        /// <summary>
        /// 按字符长加掩码
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maskcode">掩码</param>
        /// <param name="startIndex">加码开始位置</param>
        /// <param name="len">加码的长度</param>
        /// <param name="count">掩码的字符个数</param>
        /// <returns></returns>
        public static string MaskStr(this string str, int startIndex, int len, string maskcode = "...", int count = 1)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            if (str.Length < startIndex + 1)
                return str;
            StringBuilder maskSbr = new StringBuilder();
            for (int i = 0; i < count; i++)
                maskSbr.Append(maskcode);
            return str.Remove(startIndex, len).Insert(startIndex, maskSbr.ToString());
        }

        /// <summary>
        /// 按字节长度加掩码
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="maskcode">掩码</param>
        /// <param name="len">限制显示字节长度</param>
        /// <returns></returns>
        public static string MaskByte(this string str, string maskcode, int len = 1)
        {
            if (string.IsNullOrWhiteSpace(str))
                return string.Empty;
            //字节长度
            int length = Encoding.Default.GetBytes(str).Length;
            if (length <= len)
                return str;
            //记录长度
            int charIen = 0;
            //记录字符位置
            int index = 0;

            for (int i = 0; i < str.Length; i++)
            {
                if (Convert.ToInt32(str[i]) > 255)
                    charIen += 2;
                else
                    charIen += 1;
                if (charIen > len)
                {
                    index = i;
                    break;
                }
            }
            string newStr = str.Substring(0, index);
            return string.Concat(newStr, maskcode);
        }

        /// <summary>
        /// 字节长度超出范围后显示省略号
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="limitlength">限制长度</param>
        /// <param name="maskcode">掩码字符</param>
        /// <returns></returns>
        public static string OverflowHidden(this string str, int limitlength, string maskcode = "...")
        {
            return str.MaskByte(maskcode, limitlength);
        }
        /// <summary>
        /// 字符长度超出范围后显示省略号
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="limitlength">限制长度</param>
        /// <param name="maskcode">掩码字符</param>
        /// <returns></returns>
        public static string OverflowHiddenByStr(this string str, int limitlength, string maskcode = "...")
        {
            return str.MaskStr(limitlength, str.Length - limitlength);
        }
        /// <summary>
        /// 判断非空
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNullOrWhiteSpace(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// 判断非空
        /// </summary>
        /// <param name="str"></param>
        /// <param name="canSpace">容许有空白</param>
        /// <returns></returns> 
        public static bool IsNullOrWhiteSpace(this string str, bool canSpace = true)
        {
            return canSpace ? string.IsNullOrEmpty(str) : string.IsNullOrWhiteSpace(str);
        }
        /// <summary>
        /// string转long，默认0
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long ToLong(this string str)
        {
            long ret;
            if (str.IsNullOrWhiteSpace())
                return 0;
            long.TryParse(str, out ret);
            return ret;
        }
        /// <summary>
        /// 字符串转Decimal
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value">默认为0</param>
        /// <returns></returns>
        public static decimal SafeToDecimal(this string str, decimal value = 0)
        {
            if (str.IsNullOrWhiteSpace())
                return value;
            decimal.TryParse(str, out value);
            return value;
        }
        /// <summary>
        /// 字符串转Int
        /// </summary>
        /// <param name="str"></param>
        /// <param name="value">默认为0</param>
        /// <returns></returns>
        public static int ToInt(this string str, int value = 0)
        {
            if (str.IsNullOrWhiteSpace())
                return value;
            int.TryParse(str, out value);
            return value;
        }
        /// <summary>
        /// 字符串转Int
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static long SafeToLong(this string str)
        {
            long value = 0;
            if (str.IsNullOrWhiteSpace())
                return value;
            long.TryParse(str, out value);
            return value;
        }
        /// <summary>
        /// 字符串转金额
        /// </summary>
        /// <param name="str"></param>
        /// <param name="moneystr">默认人民币</param>
        /// <param name="decimals">小数</param>
        /// <returns></returns>
        public static string ToMoney(this string str, string moneystr = "¥", int decimals = 2)
        {
            //￥
            //¥
            decimal d = str.SafeToDecimal();
            return $"{moneystr}{decimal.Round(d, decimals)}";
        }

        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name="timeStamp"></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(this string timeStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        public static bool IsNumber(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return false;
            const string pattern = "^[0-9]*$";
            Regex rx = new Regex(pattern);
            return rx.IsMatch(str);
        }

        /// <summary>
        /// 截取字符串（包含中文）指定字符
        /// </summary>
        /// <param name="textString"></param>
        /// <param name="startIndex"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string SubstringExtension(this string textString, int startIndex, int length)
        {
            int len = 0;
            string textContent = "";
            for (int i = 0; i < textString.Length; i++)
            {
                byte[] byteLen = Encoding.Default.GetBytes(textString.Substring(i, 1));
                if (byteLen.Length > 1)
                {
                    len += 2; //如果长度大于1，是中文，占两个字节，+2
                }
                else
                {
                    len += 1;  //如果长度等于1，是英文，占一个字节，+1
                }
                if (len >= startIndex)
                {
                    textContent += textString.Substring(i, 1);
                }
                if (len == length + startIndex)
                {
                    break;
                }
            }
            return textContent;
        }


    }
}