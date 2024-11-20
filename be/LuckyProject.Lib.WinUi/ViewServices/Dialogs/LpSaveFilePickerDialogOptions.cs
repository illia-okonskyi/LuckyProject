using System.Collections.Generic;
using Windows.Storage;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpSaveFilePickerDialogOptions : LpPickerDialogOptions
    {
        public Dictionary<string, List<string>> FileTypeChoices { get; set; }
        public string SuggestedFileName { get; set; }
        public StorageFile SuggestedSaveFile { get; set; }
    }
}
