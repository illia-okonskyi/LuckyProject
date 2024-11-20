using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Threading;

namespace LuckyProject.Lib.Basics.Services
{
    public class CryptoService : ICryptoService
    {
        #region Internals
        private const int TempBufferSize = 128;

        private readonly IStringService stringService;
        #endregion

        #region ctor
        public CryptoService(IStringService stringService)
        {
            this.stringService = stringService;
        }
        #endregion

        #region Aes Factory method
        public Aes CreateAes(ICryptoService.AesProperties props = null)
        {
            var aes = Aes.Create();
            ApplyAesProperties(aes, props);
            return aes;
        }
        #endregion

        #region GenerateAesProperties
        public ICryptoService.AesProperties GenereteAesProperties()
        {
            using var aes = CreateAes();
            aes.GenerateKey();
            aes.GenerateIV();

            return new ICryptoService.AesProperties
            {
                Base64Key = stringService.ToBase64String(aes.Key),
                Base64Iv = stringService.ToBase64String(aes.IV)
            };
        }
        #endregion


        #region AesEncode
        public byte[] AesEncode(Aes aes, byte[] data, ICryptoService.AesProperties props = null) =>
            AesEncodeDecode(aes, data, true, props);
        public void AesEncode(
            Aes aes,
            Stream inStream,
            Stream outStream,
            ICryptoService.AesProperties props = null) =>
            AesEncodeDecode(aes, inStream, outStream, true, props);
        public Task AesEncodeAsync(
            Aes aes,
            Stream inStream,
            Stream outStream,
            ICryptoService.AesProperties props = null,
            CancellationToken cancellationToken = default) =>
            AesEncodeDecodeAsync(aes, inStream, outStream, true, props, cancellationToken);
        #endregion

        #region AesDecode
        public byte[] AesDecode(Aes aes, byte[] data, ICryptoService.AesProperties props = null) =>
            AesEncodeDecode(aes, data, false, props);
        public void AesDecode(
            Aes aes,
            Stream inStream,
            Stream outStream,
            ICryptoService.AesProperties props = null) =>
            AesEncodeDecode(aes, inStream, outStream, false, props);
        public Task AesDecodeAsync(
            Aes aes,
            Stream inStream,
            Stream outStream,
            ICryptoService.AesProperties props = null,
            CancellationToken cancellationToken = default) =>
            AesEncodeDecodeAsync(aes, inStream, outStream, false, props, cancellationToken);
        #endregion

        #region Internals
        private void ApplyAesProperties(Aes aes, ICryptoService.AesProperties props)
        {
            if (props == null)
            {
                return;
            }

            aes.Key = stringService.FromBase64String(props.Base64Key);
            aes.IV = stringService.FromBase64String(props.Base64Iv);
        }

        private void AesEncodeDecode(
            Aes aes,
            Stream inStream,
            Stream outStream,
            bool encode,
            ICryptoService.AesProperties props)
        {
            ApplyAesProperties(aes, props);
            using var encStream = new CryptoStream(
                outStream,
                encode ? aes.CreateEncryptor() : aes.CreateDecryptor(),
                CryptoStreamMode.Write);

            var tempBuf = new byte[TempBufferSize];
            long totalInBytesCount = inStream.Length;
            long totalBytesProcessed = 0;
            int bytesProcessed;
            while (totalBytesProcessed < totalInBytesCount)
            {
                bytesProcessed = inStream.Read(tempBuf, 0, TempBufferSize);
                encStream.Write(tempBuf, 0, bytesProcessed);
                totalBytesProcessed = totalBytesProcessed + bytesProcessed;
            }
        }

        private byte[] AesEncodeDecode(
            Aes aes,
            byte[] data,
            bool encode,
            ICryptoService.AesProperties props)
        {
            using var inStream = new MemoryStream(data);
            using var outStream = new MemoryStream();
            AesEncodeDecode(
                aes,
                inStream,
                outStream,
                encode,
                props);
            return outStream.ToArray();
        }

        private async Task AesEncodeDecodeAsync(
            Aes aes,
            Stream inStream,
            Stream outStream,
            bool encode,
            ICryptoService.AesProperties props,
            CancellationToken cancellationToken)
        {
            ApplyAesProperties(aes, props);
            using var encStream = new CryptoStream(
                outStream,
                encode ? aes.CreateEncryptor() : aes.CreateDecryptor(),
                CryptoStreamMode.Write);

            var tempBuf = new byte[TempBufferSize];
            long totalInBytesCount = inStream.Length;
            long totalBytesProcessed = 0;
            int bytesProcessed;
            while (totalBytesProcessed < totalInBytesCount)
            {
                bytesProcessed = await inStream.ReadAsync(
                    tempBuf,
                    0,
                    TempBufferSize,
                    cancellationToken);
                await encStream.WriteAsync(
                    tempBuf,
                    0,
                    bytesProcessed,
                    cancellationToken);
                totalBytesProcessed = totalBytesProcessed + bytesProcessed;
            }
        }
        #endregion
    }
}
