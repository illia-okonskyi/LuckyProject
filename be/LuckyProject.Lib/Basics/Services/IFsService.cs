using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    /// <summary>
    /// For most details see the <see cref="File"/> class, <see cref="Directory"/>  and 
    /// <see cref="Path"/>static members description
    /// </summary>
    public interface IFsService
    {
        #region File
        StreamReader FileOpenText(string path);
        StreamWriter FileCreateText(string path);
        StreamWriter FileAppendText(string path);

        void FileCopy(string sourceFileName, string destFileName);
        void FileCopy(string sourceFileName, string destFileName, bool overwrite);

        FileStream FileCreate(string path);
        FileStream FileCreate(string path, int bufferSize);
        FileStream FileCreate(string path, int bufferSize, FileOptions options);

        void FileDelete(string path);

        bool FileExists(string path);

        FileStream FileOpen(string path, FileStreamOptions options);
        FileStream FileOpen(string path, FileMode mode);
        FileStream FileOpen(string path, FileMode mode, FileAccess access);
        FileStream FileOpen(string path, FileMode mode, FileAccess access, FileShare share);

        SafeFileHandle FileOpenHandle(
            string path,
            FileMode mode = FileMode.Open,
            FileAccess access = FileAccess.Read,
            FileShare share = FileShare.Read,
            FileOptions options = FileOptions.None,
            long preallocationSize = 0);

        void FileSetCreationTime(string path, DateTime creationTime);
        void FileSetCreationTime(SafeFileHandle fileHandle, DateTime creationTime);
        void FileSetCreationTimeUtc(string path, DateTime creationTimeUtc);
        void FileSetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc);

        DateTime FileGetCreationTime(string path);
        DateTime FileGetCreationTime(SafeFileHandle fileHandle);
        DateTime FileGetCreationTimeUtc(string path);
        DateTime FileGetCreationTimeUtc(SafeFileHandle fileHandle);

        void FileSetLastAccessTime(string path, DateTime lastAccessTime);
        void FileSetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime);
        void FileSetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);
        void FileSetLastAccessTimeUtc(SafeFileHandle fileHandle, DateTime lastAccessTimeUtc);

        DateTime FileGetLastAccessTime(string path);
        DateTime FileGetLastAccessTime(SafeFileHandle fileHandle);
        DateTime FileGetLastAccessTimeUtc(string path);
        DateTime FileGetLastAccessTimeUtc(SafeFileHandle fileHandle);

        void FileSetLastWriteTime(string path, DateTime lastWriteTime);
        void FileSetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime);
        void FileSetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);
        void FileSetLastWriteTimeUtc(SafeFileHandle fileHandle, DateTime lastWriteTimeUtc);

        DateTime FileGetLastWriteTime(string path);
        DateTime FileGetLastWriteTime(SafeFileHandle fileHandle);
        DateTime FileGetLastWriteTimeUtc(string path);
        DateTime FileGetLastWriteTimeUtc(SafeFileHandle fileHandle);

        FileAttributes FileGetAttributes(string path);
        FileAttributes FileGetAttributes(SafeFileHandle fileHandle);

        void FileSetAttributes(string path, FileAttributes fileAttributes);
        void FileSetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes);

        [UnsupportedOSPlatform("windows")]
        UnixFileMode FileGetUnixFileMode(string path);
        [UnsupportedOSPlatform("windows")]
        UnixFileMode FileGetUnixFileMode(SafeFileHandle fileHandle);

        [UnsupportedOSPlatform("windows")]
        void FileSetUnixFileMode(string path, UnixFileMode mode);
        [UnsupportedOSPlatform("windows")]
        void FileSetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode);

        FileStream FileOpenRead(string path);
        FileStream FileOpenWrite(string path);

        string FileReadAllText(string path);
        string FileReadAllText(string path, Encoding encoding);

        void FileWriteAllText(string path, string contents);
        void FileWriteAllText(string path, string contents, Encoding encoding);

        byte[] FileReadAllBytes(string path);
        void FileWriteAllBytes(string path, byte[] bytes);

        string[] FileReadAllLines(string path);
        string[] FileReadAllLines(string path, Encoding encoding);

        IEnumerable<string> FileReadLines(string path);
        IEnumerable<string> FileReadLines(string path, Encoding encoding);

        IAsyncEnumerable<string> FileReadLinesAsync(
            string path,
            CancellationToken cancellationToken = default);
        IAsyncEnumerable<string> FileReadLinesAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        void FileWriteAllLines(string path, string[] contents);
        void FileWriteAllLines(string path, IEnumerable<string> contents);
        void FileWriteAllLines(string path, string[] contents, Encoding encoding);
        void FileWriteAllLines(string path, IEnumerable<string> contents, Encoding encoding);

        void FileAppendAllText(string path, string contents);
        void FileAppendAllText(string path, string contents, Encoding encoding);

        void FileAppendAllLines(string path, IEnumerable<string> contents);
        void FileAppendAllLines(string path, IEnumerable<string> contents, Encoding encoding);

        void FileReplace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName);
        void FileReplace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName,
            bool ignoreMetadataErrors);

        void FileMove(string sourceFileName, string destFileName);
        void FileMove(string sourceFileName, string destFileName, bool overwrite);

        [SupportedOSPlatform("windows")]
        void FileEncrypt(string path);
        [SupportedOSPlatform("windows")]
        void FileDecrypt(string path);

        Task<string> FileReadAllTextAsync(
            string path,
            CancellationToken cancellationToken = default);
        Task<string> FileReadAllTextAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        Task FileWriteAllTextAsync(
            string path,
            string contents,
            CancellationToken cancellationToken = default);
        Task FileWriteAllTextAsync(
            string path,
            string contents,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        Task<byte[]> FileReadAllBytesAsync(
            string path,
            CancellationToken cancellationToken = default);
        Task FileWriteAllBytesAsync(
            string path,
            byte[] bytes,
            CancellationToken cancellationToken = default);

        Task<string[]> FileReadAllLinesAsync(
            string path,
            CancellationToken cancellationToken = default);
        Task<string[]> FileReadAllLinesAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        Task FileWriteAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            CancellationToken cancellationToken = default);
        Task FileWriteAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        Task FileAppendAllTextAsync(
            string path,
            string contents,
            CancellationToken cancellationToken = default);
        Task FileAppendAllTextAsync(
            string path,
            string contents,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        Task FileAppendAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            CancellationToken cancellationToken = default);
        Task FileAppendAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            Encoding encoding,
            CancellationToken cancellationToken = default);

        FileSystemInfo FileCreateSymbolicLink(string path, string pathToTarget);
        FileSystemInfo FileResolveLinkTarget(string linkPath, bool returnFinalTarget);
        #endregion

        #region Directory
        DirectoryInfo DirectoryGetParent(string path);

        DirectoryInfo DirectoryCreateDirectory(string path);
        [UnsupportedOSPlatform("windows")]
        DirectoryInfo DirectoryCreateDirectory(string path, UnixFileMode unixCreateMode);

        DirectoryInfo DirectoryCreateTempSubdirectory(string prefix = null);

        bool DirectoryExists(string path);

        void DirectorySetCreationTime(string path, DateTime creationTime);
        void DirectorySetCreationTimeUtc(string path, DateTime creationTimeUtc);

        DateTime DirectoryGetCreationTime(string path);
        DateTime DirectoryGetCreationTimeUtc(string path);

        void DirectorySetLastWriteTime(string path, DateTime lastWriteTime);
        void DirectorySetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc);

        DateTime DirectoryGetLastWriteTime(string path);
        DateTime DirectoryGetLastWriteTimeUtc(string path);

        void DirectorySetLastAccessTime(string path, DateTime lastAccessTime);
        void DirectorySetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc);

        DateTime DirectoryGetLastAccessTime(string path);
        DateTime DirectoryGetLastAccessTimeUtc(string path);

        string[] DirectoryGetFiles(string path);
        string[] DirectoryGetFiles(string path, string searchPattern);
        string[] DirectoryGetFiles(string path, string searchPattern, SearchOption searchOption);
        string[] DirectoryGetFiles(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        string[] DirectoryGetDirectories(string path);
        string[] DirectoryGetDirectories(string path, string searchPattern);
        string[] DirectoryGetDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption);
        string[] DirectoryGetDirectories(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        string[] DirectoryGetFileSystemEntries(string path);
        string[] DirectoryGetFileSystemEntries(string path, string searchPattern);
        string[] DirectoryGetFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption);
        string[] DirectoryGetFileSystemEntries(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        IEnumerable<string> DirectoryEnumerateDirectories(string path);
        IEnumerable<string> DirectoryEnumerateDirectories(string path, string searchPattern);
        IEnumerable<string> DirectoryEnumerateDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption);
        IEnumerable<string> DirectoryEnumerateDirectories(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        IEnumerable<string> DirectoryEnumerateFiles(string path);
        IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern);
        IEnumerable<string> DirectoryEnumerateFiles(
            string path,
            string searchPattern,
            SearchOption searchOption);
        IEnumerable<string> DirectoryEnumerateFiles(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path);
        IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path, string searchPattern);
        IEnumerable<string> DirectoryEnumerateFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption);
        IEnumerable<string> DirectoryEnumerateFileSystemEntries(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions);

        string DirectoryGetDirectoryRoot(string path);

        string DirectoryGetCurrentDirectory();
        void DirectorySetCurrentDirectory(string path);

        void DirectoryMove(string sourceDirName, string destDirName);

        void DirectoryDelete(string path);
        void DirectoryDelete(string path, bool recursive);

        string[] DirectoryGetLogicalDrives();

        FileSystemInfo DirectoryCreateSymbolicLink(string path, string pathToTarget);
        FileSystemInfo DirectoryResolveLinkTarget(string linkPath, bool returnFinalTarget);

        #region Custom helpers
        void DirectoryEnsureCreated(string path);
        [UnsupportedOSPlatform("windows")]
        void DirectoryEnsureCreated(string path, UnixFileMode unixCreateMode);

        void DirectoryEnsureDeleted(string path);
        void DirectoryEnsureDeleted(string path, bool recursive);
        #endregion
        #endregion

        #region Path
        char DirectorySeparatorChar { get; }
        char AltDirectorySeparatorChar { get; }
        char VolumeSeparatorChar { get; }
        char PathSeparator { get; }

        string PathChangeExtension(string path, string extension);

        bool PathExists(string path);
        string PathGetDirectoryName(string path);

        ReadOnlySpan<char> PathGetDirectoryName(ReadOnlySpan<char> path);

        string PathGetExtension(string path);
        ReadOnlySpan<char> PathGetExtension(ReadOnlySpan<char> path);

        string PathGetFileName(string path);
        ReadOnlySpan<char> PathGetFileName(ReadOnlySpan<char> path);

        string PathGetFileNameWithoutExtension(string path);
        ReadOnlySpan<char> PathGetFileNameWithoutExtension(ReadOnlySpan<char> path);

        string PathGetRandomFileName();

        bool PathIsPathFullyQualified(string path);
        bool PathIsPathFullyQualified(ReadOnlySpan<char> path);

        bool PathHasExtension(string path);
        bool PathHasExtension(ReadOnlySpan<char> path);

        string PathCombine(string path1, string path2);
        string PathCombine(string path1, string path2, string path3);
        string PathCombine(string path1, string path2, string path3, string path4);
        string PathCombine(params string[] paths);

        string PathJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2);
        string PathJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2, ReadOnlySpan<char> path3);
        string PathJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            ReadOnlySpan<char> path3,
            ReadOnlySpan<char> path4);

        string PathJoin(string path1, string path2);
        string PathJoin(string path1, string path2, string path3);
        string PathJoin(string path1, string path2, string path3, string path4);
        string PathJoin(params string[] paths);

        bool PathTryJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            Span<char> destination,
            out int charsWritten);
        bool PathTryJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            ReadOnlySpan<char> path3,
            Span<char> destination,
            out int charsWritten);

        string PathGetRelativePath(string relativeTo, string path);

        string PathTrimEndingDirectorySeparator(string path);
        ReadOnlySpan<char> PathTrimEndingDirectorySeparator(ReadOnlySpan<char> path);
        bool PathEndsInDirectorySeparator(ReadOnlySpan<char> path);
        bool PathEndsInDirectorySeparator(string path);

        #region Custom helpers
        string PathNormilizeDirectorySeparators(string path);

        string PathPrefixFileName(string path, string prefix, string separator = "-");
        string PathSuffixFileName(string path, string suffix, string separator = "-");
        #endregion
        #endregion
    }

}
