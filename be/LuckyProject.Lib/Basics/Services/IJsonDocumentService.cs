using LuckyProject.Lib.Basics.LiveObjects;
using LuckyProject.Lib.Basics.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IJsonDocumentService
    {
        #region Reading
        Task<JsonDocument<TDocument>> ReadDocumentAsync<TDocument>(
            string s,
            string targetDocType,
            int targetVersion,
            List<IJsonDocumentMigrator> migrators = null,
            List<JsonConverter> jsonConverters = null,
            CancellationToken cancellationToken = default);
        Task<JsonDocument<TDocument>> ReadDocumentFromFileAsync<TDocument>(
             string filePath,
             string targetDocType,
             int targetVersion,
             List<IJsonDocumentMigrator> migrators = null,
             List<JsonConverter> jsonConverters = null,
             CancellationToken cancellationToken = default);
        Task<JsonDocument<TDocument>> ReadEncryptedDocumentAsync<TDocument>(
            byte[] data,
            Aes aes,
            string targetDocType,
            int targetVersion,
            List<IJsonDocumentMigrator> migrators = null,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null,
            CancellationToken cancellationToken = default);
        Task<JsonDocument<TDocument>> ReadEncryptedDocumentFromFileAsync<
            TDocument>(
                string filePath,
                Aes aes,
                string targetDocType,
                int targetVersion,
                List<IJsonDocumentMigrator> migrators = null,
                List<JsonConverter> jsonConverters = null,
                ICryptoService.AesProperties aesProps = null,
                CancellationToken cancellationToken = default);
        #endregion

        #region Writing
        string WriteDocument<TDocument>(
            JsonDocument<TDocument> document,
            bool indented = true,
            List<JsonConverter> jsonConverters = null);
        Task WriteDocumentToFileAsync<TDocument>(
            JsonDocument<TDocument> document,
            string filePath,
            bool indented = true,
            List<JsonConverter> jsonConverters = null,
            CancellationToken cancellationToken = default);
        byte[] WriteEncryptedDocument<TDocument>(
            JsonDocument<TDocument> document,
            Aes aes,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null);
        Task WriteEncryptedDocumentToFileAsync<TDocument>(
            JsonDocument<TDocument> document,
            string filePath,
            Aes aes,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
