using LuckyProject.Lib.Basics.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class StringExtensions
    {
        public static string ToNullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        public static string ToEmptyIfNull(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s;
        }

        public static SecureString ToSecureString(this string s)
        {
            s = s.ToEmptyIfNull();
            var ss = new SecureString();
            foreach(char c in s)
            {
                ss.AppendChar(c);
            }
            return ss;
        }

        private static readonly Regex SlashesRegex = new(@"\\|/");
        public static string ToNormalizedPath(this string s)
        {
            return SlashesRegex.Replace(s, $"{Path.DirectorySeparatorChar}");
        }

        private static readonly Regex NewlineRegex = new(@"\r?\n");
        public static string ToNormalizedNewlines(this string s)
        {
            return NewlineRegex.Replace(s, Environment.NewLine);
        }

        public static List<string> SplitByLines(this string s)
        {
            return NewlineRegex.Split(s).ToList();
        }

        public static string ToSurrounded(this string s, string begin, string end)
        {
            var sb = new StringBuilder(begin);
            sb.Append(s);
            sb.Append(end);
            return sb.ToString();
        }

        public static string ToSurrounded(this string s, string wrap)
        {
            return s.ToSurrounded(wrap, wrap);
        }

        public static string ToSurrounded(this string s, CommonStringSurround surround)
        {
            (var begin, var end) = surround switch
            {
                CommonStringSurround.Quotes => ("\"", "\""),
                CommonStringSurround.SingleQuotes => ("'", "'"),
                CommonStringSurround.BackQuotes => ("`", "`"),
                CommonStringSurround.Brackets => ("(", ")"),
                CommonStringSurround.Braces => ("{", "}"),
                CommonStringSurround.SquareBrackets => ("[", "]"),
                CommonStringSurround.TriangleBrackets => ("<", ">"),
                _ => throw new ArgumentException($"Unexpeceted {nameof(CommonStringSurround)}")
            };

            return s.ToSurrounded(begin, end);
        }
    }
}
