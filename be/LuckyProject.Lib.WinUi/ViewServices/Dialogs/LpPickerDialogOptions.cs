using Windows.Storage.Pickers;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpPickerDialogOptions
    {
        public nint? Hwnd { get; set; }
        public string SettingsIdentifier { get; set; }
        public PickerViewMode ViewMode { get; set; } = PickerViewMode.List;
        public string CommitButtonText { get; set; }
        public PickerLocationId? SuggestedStartLocation { get; set; }
    }
}
