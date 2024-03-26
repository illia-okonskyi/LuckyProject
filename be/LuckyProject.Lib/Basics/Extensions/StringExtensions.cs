using System.Security;

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
    }
}
