using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IXmlService
    {
        #region GetEncoding
        Encoding GetXmlEncoding(string xml);
        Task<Encoding> GetXmlEncodingAsync(string xml);
        Encoding GetXmlFileEncoding(string path);
        Task<Encoding> GetXmlFileEncodingAsync(string path);
        #endregion

        #region Serialization
        string Serialize(
            object value,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null);
        void SerializeToFile(
            object value,
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null);
        #endregion

        #region Deserialization
        object Deserialize(
            Type t,
            string s,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null);
        T Deserialize<T>(string s, Encoding e = null, IEnumerable<Type> extraTypes = null);
        object DeserializeFromFile(
            Type t,
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null);
        T DeserializeFromFile<T>(
            string path,
            Encoding e = null,
            IEnumerable<Type> extraTypes = null);
        #endregion
    }
}
