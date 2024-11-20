using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public interface ILpZipArchive : IDisposable
    {
        #region Properties
        ZipArchiveMode Mode { get; }
        bool IsReadOnly { get; }
        #endregion

        #region Entries
        public class EntryMetadata
        {
            public string Name { get; init; }
            public string FullName { get; init; }
            public uint Crc32 { get; init; }
            public long? Length { get; init; }
            public long? CompressedLength { get; init; }
            public DateTimeOffset? LastWriteTime { get; init; }
        }

        List<string> GetEntries();
        EntryMetadata GetEntryMetadata(string entryFullName);
        #endregion

        #region AddEntryAsync
        Task AddEntryAsync(
            string entryFullName,
            Stream data,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default);
        Task AddEntryAsync(
            string entryFullName,
            byte[] data,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default);
        Task AddEntryAsync(
            string entryFullName,
            string filePath,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default);
        #endregion

        #region ExtractEntryAsync
        Task ExtractEntryAsync(
            string entryFullName,
            Stream output,
            CancellationToken cancellationToken = default);
        Task<byte[]> ExtractEntryAsync(
            string entryFullName,
            CancellationToken cancellationToken = default);
        Task ExtractEntryAsync(
            string entryFullName,
            string filePath,
            CancellationToken cancellationToken = default);
        #endregion

        #region Directories 
        Task AddDirectoryAsync(
            string dirPath,
            bool recursive = true,
            CompressionLevel compressionLevel = CompressionLevel.Optimal,
            CancellationToken cancellationToken = default);
        Task ExtractToDirectoryAsync(
            string targetDirPath,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
