using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class NullableExtensions
    {
        #region NullIfDefault
        public static bool? NullIfDefault(this bool v) => v == default ? null : v;
        public static byte? NullIfDefault(this byte v) => v == default ? null :v;
        public static sbyte? NullIfDefault(this sbyte v) => v == default ? null : v;
        public static char? NullIfDefault(this char v) => v == default ? null : v;
        public static short? NullIfDefault(this short v) => v == default ? null : v;
        public static ushort? NullIfDefault(this ushort v) => v == default ? null : v;
        public static int? NullIfDefault(this int v) => v == default ? null : v;
        public static uint? NullIfDefault(this uint v) => v == default ? null : v;
        public static long? NullIfDefault(this long v) => v == default ? null : v;
        public static ulong? NullIfDefault(this ulong v) => v == default ? null : v;
        public static decimal? NullIfDefault(this decimal v) => v == default ? null : v;
        public static T? NullIfDefault<T>(this T value)
            where T : struct
        {
            return EqualityComparer<T>.Default.Equals(value, default) ? null : value;
        }
        #endregion

        #region DefaultIfNull
        public static bool DefaultIfNull(bool? v) => v ?? default;
        public static byte DefaultIfNull(byte? v) => v ?? default;
        public static sbyte DefaultIfNull(sbyte? v) => v ?? default;
        public static char DefaultIfNull(char? v) => v ?? default;
        public static short DefaultIfNull(short? v) => v ?? default;
        public static ushort DefaultIfNull(ushort? v) => v ?? default;
        public static int DefaultIfNull(int? v) => v ?? default;
        public static uint DefaultIfNull(uint? v) => v ?? default;
        public static long DefaultIfNull(long? v) => v ?? default;
        public static ulong DefaultIfNull(ulong? v) => v ?? default;
        public static decimal DefaultIfNull(decimal? v) => v ?? default;
        public static T DefaultIfNull<T>(this T? value) where T : struct => value ?? default;
        #endregion
    }
}
