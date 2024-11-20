using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LuckyProject.Lib.WinUi.Models
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum LpWindowState
    {
        Normal,
        Maximized,
        Minimized,
    };

    public class LpWindowStatePositionSize
    {
        public LpWindowState State { get; set; }
        public int? X { get; set; }
        public int? Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
