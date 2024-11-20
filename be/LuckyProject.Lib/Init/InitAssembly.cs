using LuckyProject.Lib.Imports;
using System.Reflection;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Init
{
    public static class InitAssembly
    {
        public static void Init()
        {
            NativeLibrary.SetDllImportResolver(
                Assembly.GetExecutingAssembly(),
                LpDllImportResolver.Resolve);
        }
    }
}
