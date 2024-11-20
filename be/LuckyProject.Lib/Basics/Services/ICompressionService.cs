using LuckyProject.Lib.Basics.LiveObjects;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ICompressionService
    {
        public enum Compressor
        {
            Deflate,
            GZip,
            ZLib
        }

        #region CompressAsync
        Task CompressAsync(
            Stream input,
            Stream output,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        Task<byte[]> CompressAsync(
            byte[] data,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        Task CompressAsync(
            string inputFilePath,
            string outputFilePath,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        #endregion

        #region DecompressAsync
        Task DecompressAsync(
            Stream input,
            Stream output,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        Task<byte[]> DecompressAsync(
            byte[] data,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        Task DecompressAsync(
            string inputFilePath,
            string outputFilePath,
            Compressor compressor = Compressor.Deflate,
            CancellationToken cancellationToken = default);
        #endregion

        #region ZipArchive factory
        ILpZipArchive CreateZipArchive(Stream s);
        ILpZipArchive CreateZipArchive(string filePath);
        ILpZipArchive OpenZipArchive(Stream s, bool readOnly = true);
        ILpZipArchive OpenZipArchive(byte[] data);
        ILpZipArchive OpenZipArchive(string filePath, bool readOnly = true);
        #endregion       
    }
}
