using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;

namespace LuckyProject.Lib.Web.Models
{
    public class LpApiHttpClientRequest
    {
        public HttpMethod Method { get; set; } = HttpMethod.Get;
        public string BaseUrl { get; set; }
        public Dictionary<string, List<string>> Parameters { get; set; } = new();
        public string AccessToken { get; set; }
        public Dictionary<string, List<string>> Headers { get; set; } = new();
        public object Content { get; set; }
        public List<JsonConverter> JsonConverters { get; set; } = new();
    }
}
