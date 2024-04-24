using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Common
{
    public static class StringExtension
    {
        public static string Cut(this string s1, int maxLen)
        {
            if (s1 == null)
            {
                return string.Empty;
            }
            int len = s1.Length <= maxLen ? s1.Length : maxLen;//不能超过字符串的最大大小
            return s1[0..len];
        }
    }
}
