using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Core.Librs
{
    /// <summary>
    /// 字符串处理类
    /// </summary>
    public static class StringUitls
    {
        /// <summary>
        /// 半角转全角
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string toSBC( string source)
        {
            char[] c = source.ToCharArray();
            for (int i = 0; i <= c.Length - 1; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
        }


        /// <summary>
        /// 全角转半角
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string toDBC(string source)
        {
            char[] c = source.ToCharArray();
            for (int i = 0; i <= c.Length - 1; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new String(c);
        }
    }
}
