using System;

namespace LuckyProject.Lib.WinUi.Models
{
    public class LpAppTaskContext
    {
        public Action<string> SetStatus { get; set; }
        public Action<int> SetProgressValue { get; init; }
        public Action<bool> SetProgressIndeterminate { get; init; }
    }
}
