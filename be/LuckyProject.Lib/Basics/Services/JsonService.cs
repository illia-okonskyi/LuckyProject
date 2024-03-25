using Newtonsoft.Json;
using System;

namespace LuckyProject.Lib.Basics.Services
{
    public class JsonService : IJsonService
    {
        public Func<JsonSerializerSettings> DefaultSettings
        {
            get => JsonConvert.DefaultSettings;
            set => JsonConvert.DefaultSettings = value;
        }

        #region Serialize
        public string SerializeObject(object value) =>
            JsonConvert.SerializeObject(value);
        public string SerializeObject(object value, Formatting formatting) =>
            JsonConvert.SerializeObject(value, formatting);
        public string SerializeObject(object value, params JsonConverter[] converters) =>
            JsonConvert.SerializeObject(value, converters);
        public string SerializeObject(
            object value,
            Formatting formatting,
            params JsonConverter[] converters) =>
            JsonConvert.SerializeObject(value, formatting, converters);
        public string SerializeObject(object value, JsonSerializerSettings settings) =>
            JsonConvert.SerializeObject(value, settings);
        public string SerializeObject(object value, Type type, JsonSerializerSettings settings) =>
            JsonConvert.SerializeObject(value, type, settings);
        public string SerializeObject(
            object value,
            Formatting formatting,
            JsonSerializerSettings settings) =>
            JsonConvert.SerializeObject(value, formatting, settings);
        public string SerializeObject(
            object value,
            Type type,
            Formatting formatting,
            JsonSerializerSettings settings) =>
            JsonConvert.SerializeObject(value, type, formatting, settings);
        #endregion

        #region Deserialize
        public object DeserializeObject(string value) =>
            JsonConvert.DeserializeObject(value);
        public object DeserializeObject(string value, JsonSerializerSettings settings) =>
            JsonConvert.DeserializeObject(value, settings);
        public object DeserializeObject(string value, Type type) =>
            JsonConvert.DeserializeObject(value, type);
        public T DeserializeObject<T>(string value) =>
            JsonConvert.DeserializeObject<T>(value);
        public T DeserializeAnonymousType<T>(string value, T anonymousTypeObject) =>
            JsonConvert.DeserializeAnonymousType(value, anonymousTypeObject);
        public T DeserializeAnonymousType<T>(
            string value,
            T anonymousTypeObject,
            JsonSerializerSettings settings) =>
            JsonConvert.DeserializeAnonymousType(value, anonymousTypeObject, settings);
        public T DeserializeObject<T>(string value, params JsonConverter[] converters) =>
            JsonConvert.DeserializeObject<T>(value, converters);
        public T DeserializeObject<T>(string value, JsonSerializerSettings settings) =>
            JsonConvert.DeserializeObject<T>(value, settings);
        public object DeserializeObject(
            string value,
            Type type,
            params JsonConverter[] converters) =>
            JsonConvert.DeserializeObject(value, type, converters);
        public object DeserializeObject(
            string value,
            Type type,
            JsonSerializerSettings settings) =>
            JsonConvert.DeserializeObject(value, type, settings);
        #endregion

        #region Populate
        public void PopulateObject(string value, object target) =>
            JsonConvert.PopulateObject(value, target);
        public void PopulateObject(
            string value,
            object target,
            JsonSerializerSettings settings) =>
            JsonConvert.PopulateObject(value, target, settings);
        #endregion
    }
}
