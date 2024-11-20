using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace LuckyProject.Lib.Basics.Services
{
    public class XmlService : IXmlService
    {
        #region Internals
        private const string EncodingRootAttribute = "encoding";

        private readonly IStringService stringService;
        private readonly IFsService fsService;
        #endregion

        #region ctor
        public XmlService(
            IStringService stringService,
            IFsService fsService)
        {
            this.stringService = stringService;
            this.fsService = fsService;
        }
        #endregion

        #region GetEncoding
        public Encoding GetXmlEncoding(string xml)
        {
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(
                stringReader,
                new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });
            xmlReader.Read();
            return GetXmlEncodingByName(xmlReader.GetAttribute(EncodingRootAttribute));
        }

        public async Task<Encoding> GetXmlEncodingAsync(string xml)
        {
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(
                stringReader,
                new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Async = true
                });
            await xmlReader.ReadAsync();
            return GetXmlEncodingByName(xmlReader.GetAttribute(EncodingRootAttribute));
        }

        public Encoding GetXmlFileEncoding(string path)
        {
            using var streamReader = fsService.FileOpenText(path);
            using var xmlReader = XmlReader.Create(
                streamReader,
                new XmlReaderSettings { ConformanceLevel = ConformanceLevel.Fragment });
            xmlReader.Read();
            return GetXmlEncodingByName(xmlReader.GetAttribute(EncodingRootAttribute));
        }

        public async Task<Encoding> GetXmlFileEncodingAsync(string path)
        {
            using var streamReader = fsService.FileOpenText(path);
            using var xmlReader = XmlReader.Create(
                streamReader,
                new XmlReaderSettings
                {
                    ConformanceLevel = ConformanceLevel.Fragment,
                    Async = true
                });
            await xmlReader.ReadAsync();
            return GetXmlEncodingByName(xmlReader.GetAttribute(EncodingRootAttribute));
        }

        private Encoding GetXmlEncodingByName(string name)
        {
            return string.IsNullOrEmpty(name)
                ? stringService.DefaultEncoding
                : stringService.GetEncoding(name);
        }
        #endregion

        #region Serialization
        public string Serialize(
            object value,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null)
        {
            e ??= stringService.DefaultEncoding;
            var serializer = new XmlSerializer(
                value.GetType(),
                extraTypes.EmptyIfNull().ToArray());
            using var ms = new MemoryStream();
            using var writer = new StreamWriter(ms, e);
            serializer.Serialize(writer, value);
            writer.Close();
            return stringService.FromMemoryStream(ms, e);
        }

        public void SerializeToFile(
            object value,
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null)
        {
            e ??= stringService.DefaultEncoding;
            var serializer = new XmlSerializer(
                value.GetType(),
                extraTypes.EmptyIfNull().ToArray());
            using var fs = fsService.FileCreate(path);
            using var writer = new StreamWriter(fs, e);
            serializer.Serialize(writer, value);
            writer.Close();
        }
        #endregion

        #region Deserialization
        public object Deserialize(
            Type t,
            string s,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null)
        {
            e ??= stringService.DefaultEncoding;
            var serializer = new XmlSerializer(t, extraTypes.EmptyIfNull().ToArray());
            using var ms = new MemoryStream(stringService.GetBytes(e, s));
            using var reader = new StreamReader(ms, e);
            return serializer.Deserialize(reader);
        }

        public T Deserialize<T>(string s, Encoding e = null, IEnumerable<Type> extraTypes = null)
        {
            return (T)Deserialize(typeof(T), s, e, extraTypes);
        }

        public object DeserializeFromFile(
            Type t,
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null)
        {
            e ??= stringService.DefaultEncoding;
            var serializer = new XmlSerializer(t, extraTypes.EmptyIfNull().ToArray());
            using var fs = fsService.FileOpenRead(path);
            using var reader = new StreamReader(fs, e);
            return serializer.Deserialize(reader);
        }

        public T DeserializeFromFile<T>(
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null)
        {
            return (T)DeserializeFromFile(typeof(T), path, e, extraTypes);
        }
        #endregion
    }
}
