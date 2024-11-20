using Newtonsoft.Json;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Services
{
    public class AppSettingsServiceOptions
    {
        public string FilePath { get; set; }
        public List<JsonConverter> JsonConverters { get; set; }
    }
}
