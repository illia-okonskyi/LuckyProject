using LuckyProject.Lib.Basics.Models;
using System;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Basics.Helpers
{
    public static class RuntimeInformationHelper
    {
        #region Public interface
        public static LpOsPlatform OsPlatform { get; }
        #endregion

        #region Internals
        static RuntimeInformationHelper()
        {
            OsPlatform = GetOsPlatform();
        }

        private static bool IsFreeBsdPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
        }

        private static bool IsLinuxPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
        }

        private static bool IsOsxPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        }

        private static bool IsWindowsPlatform()
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        }

        private static LpOsPlatform GetOsPlatform()
        {
            if (IsFreeBsdPlatform())
            {
                return LpOsPlatform.FreeBsd;
            }
            if (IsLinuxPlatform())
            {
                return LpOsPlatform.Linux;
            }
            if (IsOsxPlatform())
            {
                return LpOsPlatform.Osx;
            }
            if (IsWindowsPlatform())
            {
                return LpOsPlatform.Windows;
            }

            return LpOsPlatform.Other;
        }
        #endregion
    }
}
