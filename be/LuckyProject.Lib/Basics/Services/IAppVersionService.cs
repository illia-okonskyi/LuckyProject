using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IAppVersionService
    {
        Task InitAsync(ILogger logger = null);
        string AppVersion { get; }
    }
}
