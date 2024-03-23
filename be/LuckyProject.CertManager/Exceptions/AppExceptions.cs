using System;

namespace LuckyProject.CertManager.Exceptions
{
    public class LlCertManagerAppErrorException : Exception
    {
        public int ExitCode { get; }

        public LlCertManagerAppErrorException(int exitCode, string message = null)
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
