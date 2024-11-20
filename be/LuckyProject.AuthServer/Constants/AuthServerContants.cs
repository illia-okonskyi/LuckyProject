using System;

namespace LuckyProject.AuthServer.Constants
{
    public static class AuthServerContants
    {
        public static class TimeoutPolicies
        {
            public static class Default
            {
                public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);
            }

            public static class Long
            {
                public const string Name = "Long";
                public static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
            }

        }
    }
}
