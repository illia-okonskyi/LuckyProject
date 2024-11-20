using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.LocalizationManager.ViewModels.Pages;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.LocalizationManager.Views.Pages
{
    public sealed partial class HomePage : Page
    {
        public HomePageViewModel ViewModel { get; }

        public HomePage()
        {
            ViewModel = this.InitViewModel<HomePageViewModel>();
            InitializeComponent();
        }
    }
}

