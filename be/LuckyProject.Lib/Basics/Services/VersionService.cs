using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class VersionService : IVersionService
    {
        #region Internals
        private static readonly ICryptoService.AesProperties AesProps = new()
        {
            Base64Key = "q3T7pSro5hEO2n6BSK1oA65ovCcjD+ccHxDydMdaRmc=",
            Base64Iv = "/KSdTz5iVhmP/480ki5a4w=="
        };

        private readonly IFsService fsService;
        private readonly IStringService stringService;
        private readonly ICryptoService cryptoService;
        private readonly Aes aes;
        #endregion

        #region ctor & Dispose
        public VersionService(
            IFsService fsService,
            IStringService stringService,
            ICryptoService cryptoService)
        {
            this.fsService = fsService;
            this.stringService = stringService;
            this.cryptoService = cryptoService;
            aes = cryptoService.CreateAes(AesProps);
        }

        public void Dispose()
        {
            aes.Dispose();
        }
        #endregion

        #region ReadVersionAsync
        public async Task<string> ReadVersionAsync(
            IVersionService.VersionOptions options,
            CancellationToken cancellationToken = default)
        {
            var encryptedVersion = await ReadEncryptedVersionAsync(options, cancellationToken);
            return DecryptVersion(options, encryptedVersion);
        }

        private async Task<string> ReadEncryptedVersionAsync(
            IVersionService.VersionOptions options,
            CancellationToken cancellationToken)
        {
            if (options.Source == IVersionService.VersionSource.DirectValue)
            {
                return options.DirectValue;
            }

            return await fsService.FileReadAllTextAsync(options.FilePath, cancellationToken);
        }

        private string DecryptVersion(
            IVersionService.VersionOptions options,
            string encryptedVersion)
        {
            if (!options.IsEncrypted)
            {
                return encryptedVersion;
            }

            return stringService.GetString(
                Encoding.UTF8,
                cryptoService.AesDecode(
                    aes,
                    stringService.FromBase64String(encryptedVersion)));
        }
        #endregion

        #region WriteVersionAsync
        public async Task<string> WriteVersionAsync(
            string version,
            IVersionService.VersionOptions options,
            CancellationToken cancellationToken = default)
        {
            var encryptedVersion = EncryptVersion(options, version);
            await WriteEncryptedVersionAsync(encryptedVersion, options, cancellationToken);
            return encryptedVersion;
        }

        private string EncryptVersion(IVersionService.VersionOptions options, string version)
        {
            if (!options.IsEncrypted)
            {
                return version;
            }

            return stringService.ToBase64String(
                cryptoService.AesEncode(
                    aes,
                    stringService.GetBytes(Encoding.UTF8, version)));
        }

        private async Task WriteEncryptedVersionAsync(
            string encryptedVersion,
            IVersionService.VersionOptions options,
            CancellationToken cancellationToken)
        {
            if (options.Source == IVersionService.VersionSource.DirectValue)
            {
                options.DirectValue = encryptedVersion;
                return;
            }

            await fsService.FileWriteAllTextAsync(
                options.FilePath,
                encryptedVersion,
                cancellationToken);
        }
        #endregion
    }
}
