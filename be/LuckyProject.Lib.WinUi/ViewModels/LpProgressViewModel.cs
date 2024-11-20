using CommunityToolkit.Mvvm.ComponentModel;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    public partial class LpProgressViewModel : ObservableObject
    {
        [ObservableProperty]
        private int value;

        [ObservableProperty]
        private bool isIndeterminate;
    }
}
