using LuckyProject.Lib.Basics.Models;
using System.Runtime.InteropServices;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IRuntimeInformationService
    {
        Architecture OsArchitecture { get; }
        Architecture ProcessArchitecture { get; }
        string OsDescription { get; }
        string FrameworkDescription { get; }
        string RuntimeIdentifier { get; }
        LpOsPlatform OsPlatform { get; }
    }
}
