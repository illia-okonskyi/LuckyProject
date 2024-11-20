using LuckyProject.Lib.Basics.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class AppSettingsService<TSettings> : IAppSettingsService<TSettings>
        where TSettings : class, new()
    {
        #region Internals & ctor & Dispose
        private static readonly ICryptoService.AesProperties AesProps = new()
        {
            Base64Key = "q3T7pSro5hEO2n6BSK1oA65ovCcjD+ccHxDydMdaRmc=",
            Base64Iv = "/KSdTz5iVhmP/480ki5a4w=="
        };
        private const string DocType = "lp.doc.generic-app-settings";
        private const int DocVersion = 1;

        private readonly AppSettingsServiceOptions options;
        private readonly IJsonDocumentService jsonDocumentService;
        private readonly ILogger logger;
        private readonly Aes aes;

        public AppSettingsService(
            IOptions<AppSettingsServiceOptions> options,
            IJsonDocumentService jsonDocumentService,
            ILogger<AppSettingsService<TSettings>> logger,
            ICryptoService cryptoService)
        {
            this.options = options.Value;
            this.jsonDocumentService = jsonDocumentService;
            this.logger = logger;
            aes = cryptoService.CreateAes(AesProps);
        }

        public void Dispose()
        {
            aes.Dispose();
        }
        #endregion

        #region Interface impl
        public TSettings CurrentSettings { get; private set; } = new();

        public async Task LoadAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var doc = await jsonDocumentService.ReadEncryptedDocumentFromFileAsync<TSettings>(
                    options.FilePath,
                    aes,
                    DocType,
                    DocVersion,
                    null,
                    options.JsonConverters,
                    null,
                    cancellationToken);
                CurrentSettings = doc.Document;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to load settings");
            }
        }

        public async Task SaveAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await jsonDocumentService.WriteEncryptedDocumentToFileAsync(
                    new JsonDocument<TSettings>
                    {
                        DocType = DocType,
                        DocVersion = DocVersion,
                        Document = CurrentSettings
                    },
                    options.FilePath,
                    aes,
                    options.JsonConverters,
                    null,
                    cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to save settings");
            }
        }
        #endregion
    }
}
