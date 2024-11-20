using LuckyProject.Lib.Basics.Exceptions;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace LuckyProject.AuthServer.Helpers
{
    public static class AuthServerThrowHelper
    {
        [DoesNotReturn]
        public static void ThrowValidationErrorException(
            string path,
            string code,
            Dictionary<string, string> details = null)
        {
            throw new LpValidationErrorException(new()
            {
                IsCancelled = false,
                Errors = new() { new() { Path = path, Code = code, Details = details ?? new()} }
            });
        }
    }
}
