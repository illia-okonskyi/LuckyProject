using Microsoft.Win32.SafeHandles;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LuckyProject.Lib.Basics.Extensions;
using System.Text.RegularExpressions;

namespace LuckyProject.Lib.Basics.Services
{

    public class FsService : IFsService
    {
        #region File
        public StreamReader FileOpenText(string path) =>
            File.OpenText(path);
        public StreamWriter FileCreateText(string path) =>
            File.CreateText(path);
        public StreamWriter FileAppendText(string path) =>
            File.AppendText(path);

        public void FileCopy(string sourceFileName, string destFileName) =>
            File.Copy(sourceFileName, destFileName);
        public void FileCopy(string sourceFileName, string destFileName, bool overwrite) =>
            File.Copy(sourceFileName, destFileName, overwrite);

        public FileStream FileCreate(string path) =>
            File.Create(path);
        public FileStream FileCreate(string path, int bufferSize) =>
            File.Create(path, bufferSize);
        public FileStream FileCreate(string path, int bufferSize, FileOptions options) =>
            File.Create(path, bufferSize, options);

        public void FileDelete(string path) =>
            File.Delete(path);

        public bool FileExists(string path) =>
            File.Exists(path);

        public FileStream FileOpen(string path, FileStreamOptions options) =>
            File.Open(path, options);
        public FileStream FileOpen(string path, FileMode mode) =>
            File.Open(path, mode);
        public FileStream FileOpen(string path, FileMode mode, FileAccess access) =>
            File.Open(path, mode, access);
        public FileStream FileOpen(
            string path,
            FileMode mode,
            FileAccess access,
            FileShare share) =>
            File.Open(path, mode, access, share);

        public SafeFileHandle FileOpenHandle(
            string path,
            FileMode mode = FileMode.Open,
            FileAccess access = FileAccess.Read,
            FileShare share = FileShare.Read,
            FileOptions options = FileOptions.None,
            long preallocationSize = 0) =>
            File.OpenHandle(path, mode, access, share, options, preallocationSize);

        public void FileSetCreationTime(string path, DateTime creationTime) =>
            File.SetCreationTime(path, creationTime);
        public void FileSetCreationTime(SafeFileHandle fileHandle, DateTime creationTime) =>
            File.SetCreationTime(fileHandle, creationTime);
        public void FileSetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
            File.SetCreationTimeUtc(path, creationTimeUtc);
        public void FileSetCreationTimeUtc(SafeFileHandle fileHandle, DateTime creationTimeUtc) =>
            File.SetCreationTimeUtc(fileHandle, creationTimeUtc);

        public DateTime FileGetCreationTime(string path) =>
            File.GetCreationTime(path);
        public DateTime FileGetCreationTime(SafeFileHandle fileHandle) =>
            File.GetCreationTime(fileHandle);
        public DateTime FileGetCreationTimeUtc(string path) =>
            File.GetCreationTimeUtc(path);
        public DateTime FileGetCreationTimeUtc(SafeFileHandle fileHandle) =>
            File.GetCreationTimeUtc(fileHandle);

        public void FileSetLastAccessTime(string path, DateTime lastAccessTime) =>
            File.SetLastAccessTime(path, lastAccessTime);
        public void FileSetLastAccessTime(SafeFileHandle fileHandle, DateTime lastAccessTime) =>
            File.SetLastAccessTime(fileHandle, lastAccessTime);
        public void FileSetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
            File.SetLastAccessTimeUtc(path, lastAccessTimeUtc);
        public void FileSetLastAccessTimeUtc(
            SafeFileHandle fileHandle,
            DateTime lastAccessTimeUtc) =>
            File.SetLastAccessTimeUtc(fileHandle, lastAccessTimeUtc);

        public DateTime FileGetLastAccessTime(string path) =>
            File.GetLastAccessTime(path);
        public DateTime FileGetLastAccessTime(SafeFileHandle fileHandle) =>
            File.GetLastAccessTime(fileHandle);
        public DateTime FileGetLastAccessTimeUtc(string path) =>
            File.GetLastAccessTimeUtc(path);
        public DateTime FileGetLastAccessTimeUtc(SafeFileHandle fileHandle) =>
            File.GetLastAccessTimeUtc(fileHandle);

        public void FileSetLastWriteTime(string path, DateTime lastWriteTime) =>
            File.SetLastWriteTime(path, lastWriteTime);
        public void FileSetLastWriteTime(SafeFileHandle fileHandle, DateTime lastWriteTime) =>
            File.SetLastWriteTime(fileHandle, lastWriteTime);
        public void FileSetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
            File.SetLastWriteTimeUtc(path, lastWriteTimeUtc);
        public void FileSetLastWriteTimeUtc(
            SafeFileHandle fileHandle,
            DateTime lastWriteTimeUtc) =>
            File.SetLastWriteTimeUtc(fileHandle, lastWriteTimeUtc);

        public DateTime FileGetLastWriteTime(string path) =>
            File.GetLastWriteTime(path);
        public DateTime FileGetLastWriteTime(SafeFileHandle fileHandle) =>
            File.GetLastWriteTime(fileHandle);
        public DateTime FileGetLastWriteTimeUtc(string path) =>
            File.GetLastWriteTimeUtc(path);
        public DateTime FileGetLastWriteTimeUtc(SafeFileHandle fileHandle) =>
            File.GetLastWriteTimeUtc(fileHandle);

        public FileAttributes FileGetAttributes(string path) =>
            File.GetAttributes(path);
        public FileAttributes FileGetAttributes(SafeFileHandle fileHandle) =>
            File.GetAttributes(fileHandle);

        public void FileSetAttributes(string path, FileAttributes fileAttributes) =>
            File.SetAttributes(path, fileAttributes);
        public void FileSetAttributes(SafeFileHandle fileHandle, FileAttributes fileAttributes) =>
            File.SetAttributes(fileHandle, fileAttributes);

        [UnsupportedOSPlatform("windows")]
        public UnixFileMode FileGetUnixFileMode(string path) =>
            File.GetUnixFileMode(path);
        [UnsupportedOSPlatform("windows")]
        public UnixFileMode FileGetUnixFileMode(SafeFileHandle fileHandle) =>
            File.GetUnixFileMode(fileHandle);

        [UnsupportedOSPlatform("windows")]
        public void FileSetUnixFileMode(string path, UnixFileMode mode) =>
            File.SetUnixFileMode(path, mode);
        [UnsupportedOSPlatform("windows")]
        public void FileSetUnixFileMode(SafeFileHandle fileHandle, UnixFileMode mode) =>
            File.SetUnixFileMode(fileHandle, mode);

        public FileStream FileOpenRead(string path) =>
            File.OpenRead(path);
        public FileStream FileOpenWrite(string path) =>
            File.OpenWrite(path);

        public string FileReadAllText(string path) =>
            File.ReadAllText(path);
        public string FileReadAllText(string path, Encoding encoding) =>
            File.ReadAllText(path, encoding);

        public void FileWriteAllText(string path, string contents) =>
            File.WriteAllText(path, contents);
        public void FileWriteAllText(string path, string contents, Encoding encoding) =>
            File.WriteAllText(path, contents, encoding);

        public byte[] FileReadAllBytes(string path) =>
            File.ReadAllBytes(path);
        public void FileWriteAllBytes(string path, byte[] bytes) =>
            File.WriteAllBytes(path, bytes);

        public string[] FileReadAllLines(string path) =>
            File.ReadAllLines(path);
        public string[] FileReadAllLines(string path, Encoding encoding) =>
            File.ReadAllLines(path, encoding);

        public IEnumerable<string> FileReadLines(string path) =>
            File.ReadLines(path);
        public IEnumerable<string> FileReadLines(string path, Encoding encoding) =>
            File.ReadLines(path, encoding);

        public IAsyncEnumerable<string> FileReadLinesAsync(
            string path,
            CancellationToken cancellationToken = default) =>
            File.ReadLinesAsync(path, cancellationToken);
        public IAsyncEnumerable<string> FileReadLinesAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.ReadLinesAsync(path, encoding, cancellationToken);

        public void FileWriteAllLines(string path, string[] contents) =>
            File.WriteAllLines(path, contents);
        public void FileWriteAllLines(string path, IEnumerable<string> contents) =>
            File.WriteAllLines(path, contents);
        public void FileWriteAllLines(string path, string[] contents, Encoding encoding) =>
            File.WriteAllLines(path, contents, encoding);
        public void FileWriteAllLines(
            string path,
            IEnumerable<string> contents,
            Encoding encoding) =>
            File.WriteAllLines(path, contents, encoding);

        public void FileAppendAllText(string path, string contents) =>
            File.AppendAllText(path, contents);
        public void FileAppendAllText(string path, string contents, Encoding encoding) =>
            File.AppendAllText(path, contents, encoding);

        public void FileAppendAllLines(string path, IEnumerable<string> contents) =>
            File.AppendAllLines(path, contents);
        public void FileAppendAllLines(
            string path,
            IEnumerable<string> contents,
            Encoding encoding) =>
            File.AppendAllLines(path, contents, encoding);

        public void FileReplace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName) =>
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);
        public void FileReplace(
            string sourceFileName,
            string destinationFileName,
            string destinationBackupFileName,
            bool ignoreMetadataErrors) =>
            File.Replace(sourceFileName, destinationFileName, destinationBackupFileName);

        public void FileMove(string sourceFileName, string destFileName) =>
            File.Move(sourceFileName, destFileName);
        public void FileMove(string sourceFileName, string destFileName, bool overwrite) =>
            File.Move(sourceFileName, destFileName, overwrite);

        [SupportedOSPlatform("windows")]
        public void FileEncrypt(string path) =>
            File.Encrypt(path);
        [SupportedOSPlatform("windows")]
        public void FileDecrypt(string path) =>
            File.Decrypt(path);

        public Task<string> FileReadAllTextAsync(
            string path,
            CancellationToken cancellationToken = default) =>
            File.ReadAllTextAsync(path, cancellationToken);
        public Task<string> FileReadAllTextAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.ReadAllTextAsync(path, encoding, cancellationToken);

        public Task FileWriteAllTextAsync(
            string path,
            string contents,
            CancellationToken cancellationToken = default) =>
            File.WriteAllTextAsync(path, contents, cancellationToken);
        public Task FileWriteAllTextAsync(
            string path,
            string contents,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.WriteAllTextAsync(path, contents, encoding, cancellationToken);

        public Task<byte[]> FileReadAllBytesAsync(
            string path,
            CancellationToken cancellationToken = default) =>
            File.ReadAllBytesAsync(path, cancellationToken);
        public Task FileWriteAllBytesAsync(
            string path,
            byte[] bytes,
            CancellationToken cancellationToken = default) =>
            File.WriteAllBytesAsync(path, bytes, cancellationToken);

        public Task<string[]> FileReadAllLinesAsync(
            string path,
            CancellationToken cancellationToken = default) =>
            File.ReadAllLinesAsync(path, cancellationToken);
        public Task<string[]> FileReadAllLinesAsync(
            string path,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.ReadAllLinesAsync(path, encoding, cancellationToken);

        public Task FileWriteAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            CancellationToken cancellationToken = default) =>
            File.WriteAllLinesAsync(path, contents, cancellationToken);
        public Task FileWriteAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.WriteAllLinesAsync(path, contents, encoding, cancellationToken);

        public Task FileAppendAllTextAsync(
            string path,
            string contents,
            CancellationToken cancellationToken = default) =>
            File.AppendAllTextAsync(path, contents, cancellationToken);
        public Task FileAppendAllTextAsync(
            string path,
            string contents,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.AppendAllTextAsync(path, contents, encoding, cancellationToken);

        public Task FileAppendAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            CancellationToken cancellationToken = default) =>
            File.AppendAllLinesAsync(path, contents, cancellationToken);
        public Task FileAppendAllLinesAsync(
            string path,
            IEnumerable<string> contents,
            Encoding encoding,
            CancellationToken cancellationToken = default) =>
            File.AppendAllLinesAsync(path, contents, encoding, cancellationToken);

        public FileSystemInfo FileCreateSymbolicLink(string path, string pathToTarget) =>
            File.CreateSymbolicLink(path, pathToTarget);
        public FileSystemInfo FileResolveLinkTarget(string linkPath, bool returnFinalTarget) =>
            File.ResolveLinkTarget(linkPath, returnFinalTarget);
        #endregion

        #region Directory
        public DirectoryInfo DirectoryGetParent(string path) =>
            Directory.GetParent(path);

        public DirectoryInfo DirectoryCreateDirectory(string path) =>
            Directory.CreateDirectory(path);
        [UnsupportedOSPlatform("windows")]
        public DirectoryInfo DirectoryCreateDirectory(string path, UnixFileMode unixCreateMode) =>
            Directory.CreateDirectory(path, unixCreateMode);

        public DirectoryInfo DirectoryCreateTempSubdirectory(string prefix = null) =>
            Directory.CreateTempSubdirectory(prefix);

        public bool DirectoryExists(string path) =>
            Directory.Exists(path);

        public void DirectorySetCreationTime(string path, DateTime creationTime) =>
            Directory.SetCreationTime(path, creationTime);
        public void DirectorySetCreationTimeUtc(string path, DateTime creationTimeUtc) =>
            Directory.SetCreationTimeUtc(path, creationTimeUtc);

        public DateTime DirectoryGetCreationTime(string path) =>
            Directory.GetCreationTime(path);
        public DateTime DirectoryGetCreationTimeUtc(string path) =>
            Directory.GetCreationTimeUtc(path);

        public void DirectorySetLastWriteTime(string path, DateTime lastWriteTime) =>
            Directory.SetLastWriteTime(path, lastWriteTime);
        public void DirectorySetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc) =>
            Directory.SetLastWriteTimeUtc(path, lastWriteTimeUtc);

        public DateTime DirectoryGetLastWriteTime(string path) =>
            Directory.GetLastWriteTime(path);
        public DateTime DirectoryGetLastWriteTimeUtc(string path) =>
            Directory.GetLastWriteTimeUtc(path);

        public void DirectorySetLastAccessTime(string path, DateTime lastAccessTime) =>
            Directory.SetLastAccessTime(path, lastAccessTime);
        public void DirectorySetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc) =>
            Directory.SetLastAccessTimeUtc(path, lastAccessTimeUtc);

        public DateTime DirectoryGetLastAccessTime(string path) =>
            Directory.GetLastAccessTime(path);
        public DateTime DirectoryGetLastAccessTimeUtc(string path) =>
            Directory.GetLastAccessTimeUtc(path);

        public string[] DirectoryGetFiles(string path) =>
            Directory.GetFiles(path);
        public string[] DirectoryGetFiles(string path, string searchPattern) =>
            Directory.GetFiles(path, searchPattern);
        public string[] DirectoryGetFiles(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.GetFiles(path, searchPattern, searchOption);
        public string[] DirectoryGetFiles(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.GetFiles(path, searchPattern, enumerationOptions);

        public string[] DirectoryGetDirectories(string path) =>
            Directory.GetDirectories(path);
        public string[] DirectoryGetDirectories(string path, string searchPattern) =>
            Directory.GetDirectories(path, searchPattern);
        public string[] DirectoryGetDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.GetDirectories(path, searchPattern, searchOption);
        public string[] DirectoryGetDirectories(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.GetDirectories(path, searchPattern, enumerationOptions);

        public string[] DirectoryGetFileSystemEntries(string path) =>
            Directory.GetFileSystemEntries(path);
        public string[] DirectoryGetFileSystemEntries(string path, string searchPattern) =>
            Directory.GetFileSystemEntries(path, searchPattern);
        public string[] DirectoryGetFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.GetFileSystemEntries(path, searchPattern, searchOption);
        public string[] DirectoryGetFileSystemEntries(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.GetFileSystemEntries(path, searchPattern, enumerationOptions);

        public IEnumerable<string> DirectoryEnumerateDirectories(string path) =>
            Directory.EnumerateDirectories(path);
        public IEnumerable<string> DirectoryEnumerateDirectories(
            string path,
            string searchPattern) =>
            Directory.EnumerateDirectories(path, searchPattern);
        public IEnumerable<string> DirectoryEnumerateDirectories(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.EnumerateDirectories(path, searchPattern, searchOption);
        public IEnumerable<string> DirectoryEnumerateDirectories(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.EnumerateDirectories(path, searchPattern, enumerationOptions);

        public IEnumerable<string> DirectoryEnumerateFiles(string path) =>
            Directory.EnumerateFiles(path);
        public IEnumerable<string> DirectoryEnumerateFiles(string path, string searchPattern) =>
            Directory.EnumerateFiles(path, searchPattern);
        public IEnumerable<string> DirectoryEnumerateFiles(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.EnumerateFiles(path, searchPattern, searchOption);
        public IEnumerable<string> DirectoryEnumerateFiles(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.EnumerateFiles(path, searchPattern, enumerationOptions);

        public IEnumerable<string> DirectoryEnumerateFileSystemEntries(string path) =>
            Directory.EnumerateFileSystemEntries(path);
        public IEnumerable<string> DirectoryEnumerateFileSystemEntries(
            string path,
            string searchPattern) =>
            Directory.EnumerateFileSystemEntries(path, searchPattern);
        public IEnumerable<string> DirectoryEnumerateFileSystemEntries(
            string path,
            string searchPattern,
            SearchOption searchOption) =>
            Directory.EnumerateFileSystemEntries(path, searchPattern, searchOption);
        public IEnumerable<string> DirectoryEnumerateFileSystemEntries(
            string path,
            string searchPattern,
            EnumerationOptions enumerationOptions) =>
            Directory.EnumerateFileSystemEntries(path, searchPattern, enumerationOptions);

        public string DirectoryGetDirectoryRoot(string path) =>
            Directory.GetDirectoryRoot(path);

        public string DirectoryGetCurrentDirectory() =>
            Directory.GetCurrentDirectory();
        public void DirectorySetCurrentDirectory(string path) =>
            Directory.SetCurrentDirectory(path);

        public void DirectoryMove(string sourceDirName, string destDirName) =>
            Directory.Move(sourceDirName, destDirName);

        public void DirectoryDelete(string path) =>
            Directory.Delete(path);
        public void DirectoryDelete(string path, bool recursive) =>
            Directory.Delete(path, recursive);

        public string[] DirectoryGetLogicalDrives() =>
            Directory.GetLogicalDrives();

        public FileSystemInfo DirectoryCreateSymbolicLink(string path, string pathToTarget) =>
            Directory.CreateSymbolicLink(path, pathToTarget);
        public FileSystemInfo DirectoryResolveLinkTarget(string linkPath, bool returnFinalTarget) =>
            Directory.ResolveLinkTarget(linkPath, returnFinalTarget);

        #region Custom helpers
        public void DirectoryEnsureCreated(string path)
        {
            if (DirectoryExists(path))
            {
                return;
            }

            DirectoryCreateDirectory(path);
        }
        [UnsupportedOSPlatform("windows")]
        public void DirectoryEnsureCreated(string path, UnixFileMode unixCreateMode)
        {
            if (DirectoryExists(path))
            {
                return;
            }

            DirectoryCreateDirectory(path, unixCreateMode);
        }

        public void DirectoryEnsureDeleted(string path)
        {
            if (!DirectoryExists(path))
            {
                return;
            }

            DirectoryDelete(path);
        }
        public void DirectoryEnsureDeleted(string path, bool recursive)
        {
            if (!DirectoryExists(path))
            {
                return;
            }

            DirectoryDelete(path, recursive);
        }
        #endregion
        #endregion

        #region Path
        public char DirectorySeparatorChar => Path.DirectorySeparatorChar;
        public char AltDirectorySeparatorChar => Path.AltDirectorySeparatorChar;
        public char VolumeSeparatorChar => Path.VolumeSeparatorChar;
        public char PathSeparator => Path.PathSeparator;

        public string PathChangeExtension(string path, string extension) =>
            Path.ChangeExtension(path, extension);

        public bool PathExists(string path) =>
            Path.Exists(path);
        public string PathGetDirectoryName(string path) =>
            Path.GetDirectoryName(path);

        public ReadOnlySpan<char> PathGetDirectoryName(ReadOnlySpan<char> path) =>
            Path.GetDirectoryName(path);

        public string PathGetExtension(string path) =>
            Path.GetExtension(path);
        public ReadOnlySpan<char> PathGetExtension(ReadOnlySpan<char> path) =>
            Path.GetExtension(path);

        public string PathGetFileName(string path) =>
            Path.GetFileName(path);
        public ReadOnlySpan<char> PathGetFileName(ReadOnlySpan<char> path) =>
            Path.GetFileName(path);

        public string PathGetFileNameWithoutExtension(string path) =>
            Path.GetFileNameWithoutExtension(path);
        public ReadOnlySpan<char> PathGetFileNameWithoutExtension(ReadOnlySpan<char> path) =>
            Path.GetFileNameWithoutExtension(path);

        public string PathGetRandomFileName() =>
            Path.GetRandomFileName();

        public bool PathIsPathFullyQualified(string path) =>
            Path.IsPathFullyQualified(path);
        public bool PathIsPathFullyQualified(ReadOnlySpan<char> path) =>
            Path.IsPathFullyQualified(path);

        public bool PathHasExtension(string path) =>
            Path.HasExtension(path);
        public bool PathHasExtension(ReadOnlySpan<char> path) =>
            Path.HasExtension(path);

        public string PathCombine(string path1, string path2) =>
            Path.Combine(path1, path2);
        public string PathCombine(string path1, string path2, string path3) =>
            Path.Combine(path1, path2, path3);
        public string PathCombine(string path1, string path2, string path3, string path4) =>
            Path.Combine(path1, path2, path3, path4);
        public string PathCombine(params string[] paths) =>
            Path.Combine(paths);

        public string PathJoin(ReadOnlySpan<char> path1, ReadOnlySpan<char> path2) =>
            Path.Join(path1, path2);
        public string PathJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            ReadOnlySpan<char> path3) =>
            Path.Join(path1, path2, path3);
        public string PathJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            ReadOnlySpan<char> path3,
            ReadOnlySpan<char> path4) =>
            Path.Join(path1, path2, path3, path4);

        public string PathJoin(string path1, string path2) =>
            Path.Join(path1, path2);
        public string PathJoin(string path1, string path2, string path3) =>
            Path.Join(path1, path2, path3);
        public string PathJoin(string path1, string path2, string path3, string path4) =>
            Path.Join(path1, path2, path3, path4);
        public string PathJoin(params string[] paths) =>
            Path.Join(paths);

        public bool PathTryJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            Span<char> destination,
            out int charsWritten) =>
            Path.TryJoin(path1, path2, destination, out charsWritten);
        public bool PathTryJoin(
            ReadOnlySpan<char> path1,
            ReadOnlySpan<char> path2,
            ReadOnlySpan<char> path3,
            Span<char> destination,
            out int charsWritten) =>
            Path.TryJoin(path1, path2, destination, out charsWritten);

        public string PathGetRelativePath(string relativeTo, string path) =>
            Path.GetRelativePath(relativeTo, path);

        public string PathTrimEndingDirectorySeparator(string path) =>
            Path.TrimEndingDirectorySeparator(path);
        public ReadOnlySpan<char> PathTrimEndingDirectorySeparator(ReadOnlySpan<char> path) =>
            Path.TrimEndingDirectorySeparator(path);
        public bool PathEndsInDirectorySeparator(ReadOnlySpan<char> path) =>
            Path.EndsInDirectorySeparator(path);
        public bool PathEndsInDirectorySeparator(string path) =>
            Path.EndsInDirectorySeparator(path);

        #region Custom helpers
        private static readonly Regex SlashesRegex = new(@"\\|/");
        public string PathNormilizeDirectorySeparators(string path)
        {
            return SlashesRegex.Replace(path, $"{DirectorySeparatorChar}");
        }

        public string PathPrefixFileName(string path, string prefix, string separator = "-")
        {
            var dir = PathGetDirectoryName(path);
            var extension = PathGetExtension(path);
            var fileNameWithoutExtension = PathGetFileNameWithoutExtension(path);
            var newFileName = prefix + separator + fileNameWithoutExtension + extension;
            return PathCombine(dir, newFileName);
        }
        
        public string PathSuffixFileName(string path, string suffix, string separator = "-")
        {
            var dir = PathGetDirectoryName(path);
            var extension = PathGetExtension(path);
            var fileNameWithoutExtension = PathGetFileNameWithoutExtension(path);
            var newFileName = fileNameWithoutExtension + separator + suffix + extension;
            return PathCombine(dir, newFileName);
        }
        #endregion
        #endregion
    }
}
