using LuckyProject.Lib.Basics.LiveObjects.Processes;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Imports;
using System;
using System.Diagnostics;
using System.Runtime.Versioning;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpProcessService : LpProcessFactory, ILpProcessService
    {
        public LpProcessService(IRuntimeInformationService rtiService)
            : base(rtiService)
        { }

        [SupportedOSPlatform("linux")]
        public void PostSignalToProcess(Process process, LpSignum signal)
        {
            var osPlatform = rtiService.OsPlatform;
            if (osPlatform != LpOsPlatform.Linux)
            {
                throw new PlatformNotSupportedException("Only Linux supported");
            }

            var errorCode = LpImports.Linux.kill(process.Id, (int)signal);
            if (errorCode != 0)
            {
                throw new InvalidOperationException($"kill failed: {errorCode}");
            }
        }
    }
}
