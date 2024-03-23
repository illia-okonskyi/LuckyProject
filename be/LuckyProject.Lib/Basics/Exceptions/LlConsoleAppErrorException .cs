using System;

namespace LuckyProject.Lib.Basics.Exceptions
{
    public class LlConsoleAppErrorException : Exception
    {
        public int ExitCode { get; }

        public LlConsoleAppErrorException(int exitCode, string message = null)
            : base(BuildMessage(exitCode, message))
        {
            ExitCode = exitCode;
        }

        private static string BuildMessage(int exitCode, string message)
        {
            var result = $"Exit Code: {exitCode}";
            if (string.IsNullOrEmpty(message))
            {
                return result;
            }

            return result + $" - {message}";
        }
    }
}
