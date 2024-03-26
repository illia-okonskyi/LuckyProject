using System.IO;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ICryptoService
    {
        #region Definitions
        public class AesProperties
        {
            public string Base64Key { get; set; }
            public string Base64Iv { get; set; }
        }
        #endregion

        #region Aes Factory method
        Aes CreateAes(AesProperties props = null);
        #endregion

        #region GenerateAesProperties
        AesProperties GenereteAesProperties();
        #endregion

        #region AesEncode
        byte[] AesEncode(Aes aes, byte[] data, AesProperties props = null);
        void AesEncode(Aes aes, Stream inStream, Stream outStream, AesProperties props = null);
        Task AesEncodeAsync(
            Aes aes,
            Stream inStream,
            Stream outStream,
            AesProperties props = null,
            CancellationToken cancellationToken = default);
        #endregion

        #region AesDecode
        byte[] AesDecode(Aes aes, byte[] data, AesProperties props = null);
        void AesDecode(Aes aes, Stream inStream, Stream outStream, AesProperties props = null);
        Task AesDecodeAsync(
            Aes aes,
            Stream inStream,
            Stream outStream,
            AesProperties props = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
