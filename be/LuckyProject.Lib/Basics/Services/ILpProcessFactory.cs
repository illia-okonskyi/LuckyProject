using LuckyProject.Lib.Basics.LiveObjects.Processes;
using System.Diagnostics;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpProcessFactory
    {
        ILpConsoleProcess CreateConsoleProcess(LpProcessStartParams startParams);
        Process CreateShellExecuteProcess(LpProcessStartParams startParams);
    }
}
