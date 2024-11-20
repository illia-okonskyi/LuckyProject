using LuckyProject.Lib.Basics.Constants;

namespace LuckyProject.Lib.Basics.Exceptions
{
    public class LpExitCodeException : LpException
    {
        public int ExitCode { get; }

        public LpExitCodeException(string message, int exitCode)
            : base(message)
        {
            ExitCode = exitCode;
        }

        public LpExitCodeException(int exitCode, string customMessage = null)
            : base(BuildMessage(exitCode, customMessage))
        {
            ExitCode = exitCode;
        }

        private static string BuildMessage(int exitCode, string customMessage)
        {
            var result = $"Exit Code = {exitCode.ExitCodeToString("c")}";
            if (string.IsNullOrEmpty(customMessage))
            {
                return result;
            }

            return result + $" - {customMessage}";
        }
    }

    public class LpExitCodeException<TState> : LpExitCodeException
    {
        public TState State { get; }

        public LpExitCodeException(int exitCode, TState state = default, string message = null)
            : base(BuildMessage(exitCode, message, state), exitCode)
        {
            State = state;
        }

        private static string BuildMessage(int exitCode, string message, TState state)
        {
            var result = $"Exit Code = {exitCode.ExitCodeToString("c")}; State = {state}";
            if (string.IsNullOrEmpty(message))
            {
                return result;
            }

            return result + $" - {message}";
        }
    }
}
