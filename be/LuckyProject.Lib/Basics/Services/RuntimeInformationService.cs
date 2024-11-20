using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Models;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Basics.Services
{
    public class RuntimeInformationService : IRuntimeInformationService
    {
        public Architecture OsArchitecture => RuntimeInformation.OSArchitecture;
        public Architecture ProcessArchitecture => RuntimeInformation.ProcessArchitecture;
        public string OsDescription => RuntimeInformation.OSDescription;
        public string FrameworkDescription => RuntimeInformation.FrameworkDescription;
        public string RuntimeIdentifier => RuntimeInformation.RuntimeIdentifier;
        public LpOsPlatform OsPlatform => RuntimeInformationHelper.OsPlatform;
    }
}
