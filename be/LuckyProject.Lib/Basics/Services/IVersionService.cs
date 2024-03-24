using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IVersionService : IDisposable
    {
        #region Definitions
        public enum VersionSource
        {
            DirectValue,
            File
        }

        public class VersionOptions
        {
            public VersionSource Source { get; set; }
            public string DirectValue { get; set; }
            public string FilePath { get; set; }
            public bool IsEncrypted { get; set; }
        }
        #endregion

        /// <summary>
        /// Returns read and decrypted if required value of the version
        /// </summary>
        Task<string> ReadVersionAsync(
            VersionOptions options,
            CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns written and encrypted if required value of the version
        /// </summary>
        Task<string> WriteVersionAsync(
            string version,
            VersionOptions options,
            CancellationToken cancellationToken = default);
    }
}
