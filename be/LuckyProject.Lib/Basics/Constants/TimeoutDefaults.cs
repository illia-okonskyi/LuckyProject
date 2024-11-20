using System;
using System.Threading;

namespace LuckyProject.Lib.Basics.Constants
{
    public static class TimeoutDefaults
    {
        /// <summary>
        /// 1 ms
        /// </summary>
        public static readonly TimeSpan AlmostZero = TimeSpan.FromMilliseconds(1);
        /// <summary>
        /// 10 ms
        /// </summary>
        public static readonly TimeSpan ExtraShortest = TimeSpan.FromMilliseconds(10);
        /// <summary>
        /// 25 ms
        /// </summary>
        public static readonly TimeSpan ExtraShorter = TimeSpan.FromMilliseconds(25);
        /// <summary>
        /// 50 ms
        /// </summary>
        public static readonly TimeSpan ExtraShort = TimeSpan.FromMilliseconds(50);
        /// <summary>
        /// 100 ms
        /// </summary>
        public static readonly TimeSpan Shortest = TimeSpan.FromMilliseconds(100);
        /// <summary>
        /// 500 ms
        /// </summary>
        public static readonly TimeSpan Shorter = TimeSpan.FromMilliseconds(500);
        /// <summary>
        /// 1 s
        /// </summary>
        public static readonly TimeSpan Short = TimeSpan.FromSeconds(1);
        /// <summary>
        /// 30 s
        /// </summary>
        public static readonly TimeSpan Medium = TimeSpan.FromSeconds(30);
        /// <summary>
        /// 5 m
        /// </summary>
        public static readonly TimeSpan Long = TimeSpan.FromMinutes(5);
        /// <summary>
        /// 30 m
        /// </summary>
        public static readonly TimeSpan Longer = TimeSpan.FromMinutes(30);
        /// <summary>
        /// 3 h
        /// </summary>
        public static readonly TimeSpan Longest = TimeSpan.FromHours(3);
        /// <summary>
        /// 1 d
        /// </summary>
        public static readonly TimeSpan ExtraLong = TimeSpan.FromDays(1);
        /// <summary>
        /// 7 d
        /// </summary>
        public static readonly TimeSpan ExtraLonger = TimeSpan.FromDays(7);
        /// <summary>
        /// 30 d
        /// </summary>
        public static readonly TimeSpan ExtraLongest = TimeSpan.FromDays(30);
        public static readonly TimeSpan Infinity = Timeout.InfiniteTimeSpan;
    }
}
