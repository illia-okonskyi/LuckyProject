using System;

namespace LuckyProject.Lib.WinUi.Models
{
    public class LpAppMessage
    {
        public DateTime Timestamp { get; } = DateTime.Now;
        public string TimestampString => Timestamp.ToString("u");
        public LpAppMessageType Type { get; init; } = LpAppMessageType.Info;
        public string Category { get; init; }
        public string Message { get; init; }
    }
}
