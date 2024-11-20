using LuckyProject.Lib.Basics.LiveObjects;
using LuckyProject.Lib.Basics.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class JsonDocumentService : IJsonDocumentService
    {
        #region Internals & ctor
        private readonly IJsonService jsonService;
        private readonly IFsService fsService;
        private readonly ICryptoService cryptoService;
        private readonly IStringService stringService;

        public JsonDocumentService(
            IJsonService jsonService,
            IFsService fsService,
            ICryptoService cryptoService,
            IStringService stringService)
        {
            this.jsonService = jsonService;
            this.fsService = fsService;
            this.cryptoService = cryptoService;
            this.stringService = stringService;
        }
        #endregion

        #region Reading
        public async Task<JsonDocument<TDocument>> ReadDocumentAsync<TDocument>(
            string s,
            string targetDocType,
            int targetVersion,
            List<IJsonDocumentMigrator> migrators = null,
            List<JsonConverter> jsonConverters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(s))
            {
                return null;
            }

            if (targetVersion < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(targetVersion));
            }

            var jsonConvertersArray = jsonConverters?.ToArray() ?? Array.Empty<JsonConverter>();
            var docHeader = jsonService.DeserializeObject<JsonDocument>(
                s,
                jsonConvertersArray);
            if (!docHeader.DocType.Equals(targetDocType, StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Document type {docHeader.DocType} is unexpected ({targetVersion})");

            }
            if (docHeader.DocVersion > targetVersion)
            {
                throw new InvalidOperationException(
                    $"Document version {docHeader.DocVersion} is higher than target {targetVersion}");
            }
            if (docHeader.DocVersion == targetVersion)
            {
                return jsonService.DeserializeObject<JsonDocument<TDocument>>(
                    s,
                    jsonConvertersArray);
            }

            migrators ??= [];
            var sourceVersion = docHeader.DocVersion;
            object source = null;
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var localTargetVersion = migrators
                    .Max(m => m.FromVersion == sourceVersion && m.ToVersion <= targetVersion
                        ? m.ToVersion
                        : null);
                if (!localTargetVersion.HasValue)
                {
                    throw new InvalidOperationException(
                        $"Failed to find proper migrator from version {sourceVersion}");
                }
                var migrator = migrators
                    .First(m => m.FromVersion == sourceVersion && m.ToVersion == localTargetVersion);

                source ??= jsonService.DeserializeObject(
                    s,
                    migrator.ExpectedSourceType,
                    jsonConvertersArray);
                source = await migrator.MigrateAsync(source, cancellationToken);
                sourceVersion = migrator.ToVersion;
                if (sourceVersion == targetVersion)
                {
                    break;
                }
            }

            if (source is JsonDocument<TDocument> result)
            {
                return result;
            }

            throw new InvalidOperationException(
                $"Migrated document has unexpected type: Expected {typeof(JsonDocument<TDocument>)}; Actual {source.GetType()}");
        }

        public async Task<JsonDocument<TDocument>> ReadDocumentFromFileAsync<TDocument>(
             string filePath,
             string targetDocType,
             int targetVersion,
             List<IJsonDocumentMigrator> migrators = null,
             List<JsonConverter> jsonConverters = null,
             CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }
            
            var s = await fsService.FileReadAllTextAsync(filePath, cancellationToken);
            return await ReadDocumentAsync<TDocument>(
                s,
                targetDocType,
                targetVersion,
                migrators,
                jsonConverters,
                cancellationToken);
        }

        public async Task<JsonDocument<TDocument>> ReadEncryptedDocumentAsync<TDocument>(
            byte[] data,
            Aes aes,
            string targetDocType,
            int targetVersion,
            List<IJsonDocumentMigrator> migrators = null,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null,
            CancellationToken cancellationToken = default)
        {
            var docString = stringService.GetString(
                Encoding.UTF8,
                cryptoService.AesDecode(aes, data, aesProps));
            return await ReadDocumentAsync<TDocument>(
                docString,
                targetDocType,
                targetVersion,
                migrators,
                jsonConverters,
                cancellationToken);
        }

        public async Task<JsonDocument<TDocument>> ReadEncryptedDocumentFromFileAsync<
            TDocument>(
                string filePath,
                Aes aes,
                string targetDocType,
                int targetVersion,
                List<IJsonDocumentMigrator> migrators = null,
                List<JsonConverter> jsonConverters = null,
                ICryptoService.AesProperties aesProps = null,
                CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            var data = await fsService.FileReadAllBytesAsync(filePath, cancellationToken);
            return await ReadEncryptedDocumentAsync<TDocument>(
                data,
                aes,
                targetDocType,
                targetVersion,
                migrators,
                jsonConverters,
                aesProps,
                cancellationToken);
        }
        #endregion

        #region Writing
        public string WriteDocument<TDocument>(
            JsonDocument<TDocument> document,
            bool indented = true,
            List<JsonConverter> jsonConverters = null)
        {
            if (document is null)
            {
                return null;
            }

            var jsonConvertersArray = jsonConverters?.ToArray() ?? Array.Empty<JsonConverter>();
            return jsonService.SerializeObject(
                document,
                indented ? Formatting.Indented : Formatting.None,
                jsonConvertersArray);
        }

        public async Task WriteDocumentToFileAsync<TDocument>(
            JsonDocument<TDocument> document,
            string filePath,
            bool indented = true,
            List<JsonConverter> jsonConverters = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var s = WriteDocument(document, indented, jsonConverters);
            fsService.DirectoryEnsureCreated(fsService.PathGetDirectoryName(filePath));
            await fsService.FileWriteAllTextAsync(filePath, s, cancellationToken);
        }

        public byte[] WriteEncryptedDocument<TDocument>(
            JsonDocument<TDocument> document,
            Aes aes,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null)
        {
            var docString = WriteDocument(document, false, jsonConverters);
            var data = stringService.GetBytes(Encoding.UTF8, docString);
            return cryptoService.AesEncode(aes, data, aesProps);
        }

        public async Task WriteEncryptedDocumentToFileAsync<TDocument>(
            JsonDocument<TDocument> document,
            string filePath,
            Aes aes,
            List<JsonConverter> jsonConverters = null,
            ICryptoService.AesProperties aesProps = null,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                throw new ArgumentNullException(nameof(filePath));
            }

            var data = WriteEncryptedDocument(document, aes, jsonConverters, aesProps);
            fsService.DirectoryEnsureCreated(fsService.PathGetDirectoryName(filePath));
            await fsService.FileWriteAllBytesAsync(filePath, data, cancellationToken);
        }
        #endregion
    }
}
