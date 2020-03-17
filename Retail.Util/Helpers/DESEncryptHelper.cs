using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Retail.Util.Helpers
{
    /// <summary>
    /// 加密解密
    /// </summary>
    public static class DESEncryptHelper
    {
        private const string PrivateKey = "XiBin@?*";
        /// <summary>
        /// DES加密CBC
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <returns></returns>
        public static string DesEncrypt(string pToEncrypt)
        {
            try
            {
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);
                des.Key = Encoding.ASCII.GetBytes(PrivateKey);
                des.IV = Encoding.ASCII.GetBytes(PrivateKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                return ret.ToString();
            }
            catch (Exception ex)
            {

                return "";
            }

        }
        /// <summary>
        /// DES解密CBC
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <returns></returns>
        public static string DesDecrypt(string pToDecrypt)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            try
            {
                byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
                for (int x = 0; x < pToDecrypt.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = Encoding.ASCII.GetBytes(PrivateKey);
                des.IV = Encoding.ASCII.GetBytes(PrivateKey);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                return System.Text.Encoding.Default.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        /// <summary>
        /// DES ECB加密
        /// </summary>
        /// <param name="pToEncrypt"></param>
        /// <returns></returns>
        public static string DesEncryptECB(string pToEncrypt)
        {
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

                desProvider.Key = Encoding.ASCII.GetBytes(PrivateKey);
                desProvider.IV = desProvider.Key;

                //byte[] keyBytes = Encoding.ASCII.GetBytes(sKey);
                //byte[] keyIV = keyBytes;
                byte[] inputByteArray = Encoding.Default.GetBytes(pToEncrypt);

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateEncryptor(), CryptoStreamMode.Write);

                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                return Convert.ToBase64String(memStream.ToArray());

            }
            catch (Exception ex)
            {

                return pToEncrypt;
            }

        }
        /// <summary>
        /// DES解密ECB
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <returns></returns>
        public static string DesDecryptECB(string pToDecrypt)
        {
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();

                desProvider.Key = Encoding.ASCII.GetBytes(PrivateKey);
                desProvider.IV = desProvider.Key;

                byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(), CryptoStreamMode.Write);

                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                string dd = Encoding.Default.GetString(memStream.ToArray());
                return dd;
            }
            catch (Exception ex)
            {

                return "";
            }
        }

        /// DES解密ECB
        /// </summary>
        /// <param name="pToDecrypt"></param>
        /// <returns></returns>
        public static string DesDecryptECB2(string pToDecrypt)
        {
            try
            {
                DESCryptoServiceProvider desProvider = new DESCryptoServiceProvider();
                byte[] keys = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    keys[i] = Convert.ToByte(PrivateKey.Substring(i * 2, 2), 16);
                }
                desProvider.Key = keys;//Encoding.ASCII.GetBytes(sKey);
                desProvider.IV = desProvider.Key;

                byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);

                // java 默认的是ECB模式，PKCS5padding；c#默认的CBC模式，PKCS7padding 所以这里我们默认使用ECB方式
                desProvider.Mode = CipherMode.ECB;
                MemoryStream memStream = new MemoryStream();
                CryptoStream crypStream = new CryptoStream(memStream, desProvider.CreateDecryptor(), CryptoStreamMode.Write);

                crypStream.Write(inputByteArray, 0, inputByteArray.Length);
                crypStream.FlushFinalBlock();
                string dd = Encoding.Default.GetString(memStream.ToArray());
                return dd;
            }
            catch (Exception ex)
            {

                return "";
            }
        }
    }
}