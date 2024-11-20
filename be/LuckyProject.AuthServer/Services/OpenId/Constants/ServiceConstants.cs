using System.Text.RegularExpressions;

namespace LuckyProject.AuthServer.Services.OpenId.Constants
{
    public class ServiceConstants
    {
        public static readonly Regex MachineClientSecretRegex = new(
            @"[a-zA-Z0-9\[\{\]\}\;\:\'\""\,\<\.\>\/\?\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\\\|]{16,64}",
            RegexOptions.Compiled);
        public const string MachineClientSecretChars =
            @"qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890[{]};:'"",<.>/?`~!@#$%^&*()-_=+\|";
    }
}
