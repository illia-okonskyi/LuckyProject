using System;

namespace LuckyProject.Lib.Basics.Constants
{
    public static class ExitCodes
    {
        public const int Success = 0;
        public const int MissingArgument = 1;
        public const int ArgumentError = 2;
        public const int NotAuthorized = 3;
        public const int AuthFailed = 4;
        public const int AccessDenied = 5;
        public const int InvalidOperationOrState = 6;
        public const int TimedOut = 7;
        public const int Cancelled = 8;
        public const int Stopped = 9;
        public const int Paused = 10;
        public const int CancelledOrTimedOut = 11;
        public const int Rejected = 12;
        public const int Unknown = -1;

        /// <summary>
        /// Acceptable formats: n - numeric, s - string, c - combined: s(n)
        /// </summary>
        public static string ExitCodeToString(this int exitCode, string format = "s")
        {
            if (format.Equals("n", StringComparison.OrdinalIgnoreCase))
            {
                return exitCode.ToString();
            }

            var s = exitCode switch
            {
                Success => "Success",
                MissingArgument => "MissingArgument",
                ArgumentError => "ArgumentError",
                NotAuthorized => "NotAuthorized",
                AuthFailed => "AuthFailed",
                AccessDenied => "AccessDenied",
                InvalidOperationOrState => "InvalidOperationOrState",
                TimedOut => "TimedOut",
                Cancelled => "Cancelled",
                Stopped => "Stopped",
                Paused => "Paused",
                CancelledOrTimedOut => "CancelledOrTimedOut",
                Rejected => "Rejected",
                Unknown => "Unknown",
                _ => "UNDEFINED"
            };

            if (format.Equals("s", StringComparison.OrdinalIgnoreCase))
            {
                return s;
            }

            return $"{s}({exitCode})";
        }
    }
}
