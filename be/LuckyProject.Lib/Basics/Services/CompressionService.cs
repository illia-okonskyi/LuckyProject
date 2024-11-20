using LuckyProject.Lib.Basics.LiveObjects;
using System;
using System.IO.Compression;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace LuckyProject.Lib.Basics.Services
{
    public class CompressionService : ICompressionService
    {
        #region Internals
        private readonly IFsService fsService;
        #endregion

        #region ctor
        public CompressionService(IFsService fsService)
        {
            this.fsService = fsService;
        }
        #endregion

        #region Public interface
        #region CompressAsync
        public async Task CompressAsync(
            Stream input,
            Stream output,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var stream = CreateCompressor(compressor, output, CompressionMode.Compress);
            await input.CopyToAsync(stream, cancellationToken);
        }

        public async Task<byte[]> CompressAsync(
            byte[] data,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            await CompressAsync(input, output, compressor, cancellationToken);
            return output.ToArray();
        }

        public async Task CompressAsync(
            string inputFilePath,
            string outputFilePath,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var input = fsService.FileOpen(inputFilePath, FileMode.Open);
            using var output = fsService.FileCreate(outputFilePath);
            await CompressAsync(input, output, compressor, cancellationToken);
        }
        #endregion

        #region DecompressAsync
        public async Task DecompressAsync(
            Stream input,
            Stream output,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var decompressor = CreateCompressor(
                compressor,
                input,
                CompressionMode.Decompress);
            await decompressor.CopyToAsync(output, cancellationToken);
        }

        public async Task<byte[]> DecompressAsync(
            byte[] data,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var input = new MemoryStream(data);
            using var output = new MemoryStream();
            await DecompressAsync(input, output, compressor, cancellationToken);
            return output.ToArray();
        }

        public async Task DecompressAsync(
            string inputFilePath,
            string outputFilePath,
            ICompressionService.Compressor compressor = ICompressionService.Compressor.Deflate,
            CancellationToken cancellationToken = default)
        {
            using var input = fsService.FileOpen(inputFilePath, FileMode.Open);
            using var output = fsService.FileCreate(outputFilePath);
            await DecompressAsync(input, output, compressor, cancellationToken);
        }
        #endregion

        #region ZipArchive factory
        public ILpZipArchive CreateZipArchive(Stream s)
        {
            return new LpZipArchive(fsService, s, ZipArchiveMode.Create);
        }

        public ILpZipArchive CreateZipArchive(string filePath)
        {
            return CreateZipArchive(fsService.FileOpenWrite(filePath));
        }

        public ILpZipArchive OpenZipArchive(Stream s, bool readOnly = true)
        {
            return new LpZipArchive(
                fsService,
                s,
                readOnly ? ZipArchiveMode.Read : ZipArchiveMode.Update);
        }

        public ILpZipArchive OpenZipArchive(byte[] data)
        {
            return OpenZipArchive(new MemoryStream(data), true);
        }

        public ILpZipArchive OpenZipArchive(string filePath, bool readOnly = true)
        {
            var s = readOnly
                ? fsService.FileOpenRead(filePath)
                : fsService.FileOpen(filePath, FileMode.Open, FileAccess.ReadWrite);
            return OpenZipArchive(s, readOnly);
        }
        #endregion
        #endregion

        #region Internals
        private static Stream CreateCompressor(
            ICompressionService.Compressor compressor,
            Stream destination,
            CompressionMode mode)
        {
            return compressor switch
            {
                ICompressionService.Compressor.Deflate =>
                    new DeflateStream(destination, mode),
                ICompressionService.Compressor.GZip =>
                    new GZipStream(destination, mode),
                ICompressionService.Compressor.ZLib =>
                    new ZLibStream(destination, mode),
                _ => throw new ArgumentException("Unexpected compressor", nameof(compressor)),
            };
        }
        #endregion
    }
}
