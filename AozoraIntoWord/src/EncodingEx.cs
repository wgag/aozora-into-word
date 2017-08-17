using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace AozoraIntoWord
{
    internal static class EncodingEx
    {
        static readonly string file = @"jisx0213-2004-std.txt";
        static readonly string table;

        static EncodingEx()
        {
            using (StreamReader reader = new StreamReader(file, Encoding.UTF8))
            {
                table = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Converts the specified kuten into a character.
        /// </summary>
        /// <param name="kuten">The kuten string in the format "p-r-c", such as "1-2-3" or "1-02-03".</param>
        /// <returns></returns>
        public static string KutenToString(string kuten)
        {
            Regex regex = new Regex(@"(?<p>\d+)-(?<r>\d+)-(?<c>\d+)");
            int p = int.Parse(regex.Match(kuten).Groups["p"].Value);
            int r = int.Parse(regex.Match(kuten).Groups["r"].Value);
            int c = int.Parse(regex.Match(kuten).Groups["c"].Value);
            return KutenToString(p, r, c);
        }

        /// <summary>
        /// Converts the specified <i>kuten</i> into a character.
        /// </summary>
        /// <param name="p">The p value.</param>
        /// <param name="r">The r value.</param>
        /// <param name="c">The c value.</param>
        /// <returns></returns>
        public static string KutenToString(int p, int r, int c)
        {
            string jis = string.Format("{0}-{1}", p + 2, (r * 0x100 + c + 0x2020).ToString("X4"));
            Match match = Regex.Match(table, @"^" + jis + @"\tU+(?<ucs>.+?)\t", RegexOptions.Multiline);
            int codeUcs = Convert.ToInt32(match.Groups["ucs"].Value, 16);
            return char.ConvertFromUtf32(codeUcs);            
        }
    }
}
