using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.LocalizationManager.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.LocalizationManager.Views.Pages
{
    public sealed partial class SettingsPage : Page
    {
        public SettingsPageViewModel ViewModel { get; }

        public SettingsPage()
        {
            ViewModel = this.InitViewModel<SettingsPageViewModel>();
            InitializeComponent();
        }
    }
}
