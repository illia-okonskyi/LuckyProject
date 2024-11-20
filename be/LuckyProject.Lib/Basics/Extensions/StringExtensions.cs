using System.Security;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class StringExtensions
    {
        public static string NullIfEmpty(this string s)
        {
            return string.IsNullOrEmpty(s) ? null : s;
        }

        public static string EmptyIfNull(this string s)
        {
            return string.IsNullOrEmpty(s) ? string.Empty : s;
        }

        public static SecureString ToSecureString(this string s)
        {
            s = s.EmptyIfNull();
            var ss = new SecureString();
            foreach(char c in s)
            {
                ss.AppendChar(c);
            }
            return ss;
        }
    }
}
