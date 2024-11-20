using System;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpDialogBlockOptions
    {
        public LpDialogBlockMode Mode { get; set; }
        public TimeSpan Timeout { get; set; }
        public string CustomBlockTimeLeftHeader { get; set; }
    }
}
