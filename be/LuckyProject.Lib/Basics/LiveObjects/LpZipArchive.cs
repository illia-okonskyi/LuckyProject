using LuckyProject.Lib.Basics.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public class LpZipArchive : ILpZipArchive
    {
        #region Internals
        private readonly IFsService fsService;
        private readonly ZipArchive backend;
        #endregion

        #region Public properties
        public ZipArchiveMode Mode => backend.Mode;
        public bool IsReadOnly => Mode == ZipArchiveMode.Read;
        #endregion

        #region ctor & Dispose
        public LpZipArchive(
            IFsService fsService,
            Stream s,
            ZipArchiveMode mode)
        {
            this.fsService = fsService;
            backend = new ZipArchive(s, mode);
        }

        public void Dispose()
        {
            backend.Dispose();
        }
        #endregion

        #region Entries
        public List<string> GetEntries()
        {
            return backend.Entries.Select(e => e.FullName).ToList();
        }

        public ILpZipArchive.EntryMetadata GetEntryMetadata(string entryFullName)
        {
            var entry = backend.GetEntry(entryFullName);

            long? length = null;
            try
            {
                length = entry.Length;
            }
            catch
            {
                // swallow
            }

            long? compressedLength = null;
            try
            {
                compressedLength = entry.CompressedLength;
            }
            catch
            {
                // swallow
            }

            DateTimeOffset? lastWriteTime = null;
            try
            {
                lastWriteTime = entry.LastWriteTime;
            }
            catch
            {
                // swallow
            }

            return new ILpZipArchive.EntryMetadata
            {
                Name = entry.Name,
                FullName = entry.FullName,
                Crc32 = entry.Crc32,
                Length = length,
                CompressedLength = compressedLength,
                LastWriteTime = lastWriteTime
            };
        }
        #endregion

        #region AddEntryAsync
        public async Task AddEntryAsync(
            string entryFullName,
            Stream data,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default)
        {
            var entry = backend.CreateEntry(entryFullName, compressionLevel);
            using var entryStream = entry.Open();
            await data.CopyToAsync(entryStream, cancellationToken);
        }

        public async Task AddEntryAsync(
            string entryFullName,
            byte[] data,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default)
        {
            using var ms = new MemoryStream(data);
            await AddEntryAsync(entryFullName, ms, compressionLevel, cancellationToken);
        }

        public async Task AddEntryAsync(
            string entryFullName,
            string filePath,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default)
        {
            using var file = fsService.FileOpenRead(filePath);
            await AddEntryAsync(entryFullName, file, compressionLevel, cancellationToken);
        }
        #endregion

        #region ExtractEntryAsync
        public async Task ExtractEntryAsync(
            string entryFullName,
            Stream output,
            CancellationToken cancellationToken = default)
        {
            var entry = backend.GetEntry(entryFullName);
            using var entryStream = entry.Open();
            await entryStream.CopyToAsync(output, cancellationToken);
        }

        public async Task<byte[]> ExtractEntryAsync(
            string entryFullName,
            CancellationToken cancellationToken = default)
        {
            using var output = new MemoryStream();
            await ExtractEntryAsync(entryFullName, output, cancellationToken);
            return output.ToArray();
        }

        public async Task ExtractEntryAsync(
            string entryFullName,
            string filePath,
            CancellationToken cancellationToken = default)
        {
            using var output = fsService.FileOpenWrite(filePath);
            await ExtractEntryAsync(entryFullName, output, cancellationToken);
        }
        #endregion

        #region Directories 
        public async Task AddDirectoryAsync(
            string dirPath,
            bool recursive = true,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default)
        {
            var entriesInfo = fsService
                .DirectoryEnumerateFiles(
                    dirPath,
                    string.Empty,
                    recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(p => new
                {
                    FilePath = p,
                    EntryFullName = fsService.PathGetRelativePath(dirPath, p)
                });

            foreach (var info in entriesInfo)
            {
                await AddEntryAsync(
                    info.EntryFullName,
                    info.FilePath,
                    compressionLevel,
                    cancellationToken);
            }
        }

        public async Task ExtractToDirectoryAsync(
            string targetDirPath,
            CancellationToken cancellationToken = default)
        {
            var infos = GetEntries()
                .Select(e => new
                {
                    EntryFullName = e,
                    FilePath = fsService.PathNormilizeDirectorySeparators(
                        fsService.PathCombine(targetDirPath, e)),
                })
                .ToList();

            foreach (var info in infos)
            {
                fsService.DirectoryEnsureCreated(fsService.PathGetDirectoryName(info.FilePath));
                await ExtractEntryAsync(info.EntryFullName, info.FilePath, cancellationToken);
            }
        }
        #endregion
    }
}
