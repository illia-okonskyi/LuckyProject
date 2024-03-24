using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class AppVersionService : IAppVersionService
    {
        #region Public props
        public string AppVersion { get; private set; }
        #endregion

        #region Internals
        private readonly AppVersionServiceOptions options;
        private readonly IVersionService versionService;
        #endregion

        #region ctor
        public AppVersionService(
            IOptions<AppVersionServiceOptions> options,
            IVersionService versionService)
        {
            this.options = options.Value;
            this.versionService = versionService;
        }
        #endregion

        #region InitAsync
        public async Task InitAsync(ILogger logger = null)
        {
            logger?.LogInformation("Reading version from file {FilePath}...", options.FilePath);
            AppVersion = await versionService.ReadVersionAsync(
                new()
                {
                    Source = IVersionService.VersionSource.File,
                    FilePath = options.FilePath,
                    IsEncrypted = options.IsEncrypted
                });
            logger?.LogInformation("Version: {AppVersion}", AppVersion);
        }
        #endregion
    }
}
