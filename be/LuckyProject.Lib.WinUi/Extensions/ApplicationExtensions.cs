using LuckyProject.Lib.WinUi.EntryPoint;
using Microsoft.UI.Xaml;

namespace LuckyProject.Lib.WinUi.Extensions
{
    public static class ApplicationExtensions
    {
        public static ILpApplicationRoot GetLpApplicationRoot(this Application app)
        {
            return (app as ILpApplicationRootProvider)?.LpApplicationRoot;
        }
    }
}
