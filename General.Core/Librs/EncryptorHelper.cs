using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace General.Core.Librs
{
    public class EncryptorHelper
    {
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="sourceString"></param>
        /// <returns></returns>
        public static string GetMD5(string sourceString)
        {
            MD5 md5 = MD5.Create();
            byte[] source = md5.ComputeHash(Encoding.UTF8.GetBytes(sourceString));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < source.Length; i++)
            {
                sBuilder.Append(source[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        /// <summary>
        /// 生成Salt盐
        /// </summary>
        /// <param name="size">随机数长度，默认32字节</param>
        /// <returns></returns>
        public static string CreateSaltKey(int size = 32)
        {
            var rng = new RNGCryptoServiceProvider();
            var buff = new byte[size];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }

    }
}
