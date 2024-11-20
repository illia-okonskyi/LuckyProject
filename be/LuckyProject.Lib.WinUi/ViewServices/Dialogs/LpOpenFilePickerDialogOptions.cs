using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpOpenFilePickerDialogOptions : LpPickerDialogOptions
    {
        public List<string> FileTypeFilter { get; set; } = ["*"];
    }
}
