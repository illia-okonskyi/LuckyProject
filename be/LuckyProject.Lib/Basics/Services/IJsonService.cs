using Newtonsoft.Json;
using System;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IJsonService
    {
        Func<JsonSerializerSettings> DefaultSettings { get; set; }

        #region Serialize
        string SerializeObject(object value);
        string SerializeObject(object value, Formatting formatting);
        string SerializeObject(object value, params JsonConverter[] converters);
        string SerializeObject(
            object value,
            Formatting formatting,
            params JsonConverter[] converters);
        string SerializeObject(object value, JsonSerializerSettings settings);
        string SerializeObject(object value, Type type, JsonSerializerSettings settings);
        string SerializeObject(
            object value,
            Formatting formatting,
            JsonSerializerSettings settings);
        string SerializeObject(
            object value,
            Type type,
            Formatting formatting,
            JsonSerializerSettings settings);
        #endregion

        #region Deserialize
        object DeserializeObject(string value);
        object DeserializeObject(string value, JsonSerializerSettings settings);
        object DeserializeObject(string value, Type type);
        T DeserializeObject<T>(string value);
        T DeserializeAnonymousType<T>(string value, T anonymousTypeObject);
        T DeserializeAnonymousType<T>(
            string value,
            T anonymousTypeObject,
            JsonSerializerSettings settings);
        T DeserializeObject<T>(string value, params JsonConverter[] converters);
        T DeserializeObject<T>(string value, JsonSerializerSettings settings);
        object DeserializeObject(string value, Type type, params JsonConverter[] converters);
        object DeserializeObject(string value, Type type, JsonSerializerSettings settings);
        #endregion

        #region Populate
        void PopulateObject(string value, object target);
        void PopulateObject(string value, object target, JsonSerializerSettings settings);
        #endregion
    }
}
