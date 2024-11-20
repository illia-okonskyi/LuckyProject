using CommunityToolkit.Mvvm.ComponentModel;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    public partial class PageHeaderViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title;

        [ObservableProperty]
        private object extraContent;
    }
}
