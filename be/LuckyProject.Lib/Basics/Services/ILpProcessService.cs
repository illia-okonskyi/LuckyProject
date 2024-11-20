using LuckyProject.Lib.Basics.LiveObjects.Processes;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpProcessService : ILpProcessFactory
    {
        [SupportedOSPlatform("linux")]
        void PostSignalToProcess(Process process, LpSignum signal);
    }
}
