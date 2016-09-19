using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sharpenter.ResumeParser.ResumeProcessor.Helpers
{
    public static class StringHelper
    {
        public static String preserveAlphaNumeric(String str)
        {
            Regex rgx = new Regex(@"[^A-Za-z0-9~!#$^&*()_+|`\-=\\{}:"">?<\[\];',./ ]");
            str = rgx.Replace(str, "");
            str = str.Trim();
            return str;
        }
        public static string RemoveSpecialCharacters(string input)
        {
            return input.Substring(Regex.Match(input, @"[a-zA-Z0-9]").Index);
        }
        public static Random random = new Random((int)DateTime.Now.Ticks);
        public static string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }
    }
}
