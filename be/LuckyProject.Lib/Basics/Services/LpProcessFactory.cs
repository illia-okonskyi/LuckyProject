using LuckyProject.Lib.Basics.LiveObjects.Processes;
using System.Diagnostics;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpProcessFactory : ILpProcessFactory
    {
        protected readonly IRuntimeInformationService rtiService;

        public LpProcessFactory(IRuntimeInformationService rtiService)
        {
            this.rtiService = rtiService;
        }

        public ILpConsoleProcess CreateConsoleProcess(LpProcessStartParams startParams)
        {
            return new LpConsoleProcess(rtiService, startParams);
        }

        public Process CreateShellExecuteProcess(LpProcessStartParams startParams)
        {
            var psi = new ProcessStartInfo(startParams.FileName, startParams.Arguments)
            {
                UseShellExecute = true,
                WorkingDirectory = startParams.WorkingDir
            };
            foreach (var kvp in startParams.Environment)
            {
                psi.EnvironmentVariables.Add(kvp.Key, kvp.Value);
            }

            return new Process { StartInfo = psi };
        }
    }
}
