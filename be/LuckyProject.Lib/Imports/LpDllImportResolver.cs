using LuckyProject.Lib.Basics.Helpers;
using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Imports
{
    public static class LpDllImportResolver
    {
        public static IntPtr Resolve(
            string libraryName,
            Assembly assembly,
            DllImportSearchPath? searchPath)
        {
            var osPlatform = RuntimeInformationHelper.OsPlatform;
            if (osPlatform == Basics.Models.LpOsPlatform.Windows)
            {
                if (libraryName == "windows-kernel32.dll")
                {
                    return NativeLibrary.Load("kernel32.dll", assembly, searchPath);
                }

                return IntPtr.Zero;
            }

            if (osPlatform == Basics.Models.LpOsPlatform.Linux)
            {
                if (libraryName == "linux-libc")
                {
                    return NativeLibrary.Load("libc", assembly, searchPath);
                }

                return IntPtr.Zero;

            }

            return IntPtr.Zero;
        }
    }
}
