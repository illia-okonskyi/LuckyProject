using System;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToString(
            this object o,
            string format,
            IFormatProvider formatProvider = null)
        {
            if (o == null)
            {
                return null;
            }

            if (o is string s)
            {
                return s.ToString(formatProvider);
            }

            if (o is bool b)
            {
                return b.ToString(formatProvider);
            }

            if (o is IFormattable formattable)
            {
                return formattable.ToString(format, formatProvider);
            }

            return o.ToString();
        }
    }
}
