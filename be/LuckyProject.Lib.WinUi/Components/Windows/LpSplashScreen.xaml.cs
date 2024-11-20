using LuckyProject.Lib.WinUi.ViewModels.Windows;
using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using WinUIEx;

namespace LuckyProject.Lib.WinUi.Components.Windows
{
    public sealed partial class LpSplashScreen : SplashScreen
    {
        public LpSplashScreenViewModel ViewModel { get; }

        public LpSplashScreen(LpSplashScreenViewModel viewModel, Window window)
            : base(window)
        {
            ViewModel = viewModel;
            InitializeComponent();
            Completed += LpSplashScreen_Completed;
            Closed += LpSplashScreen_Closed;
        }

        private void LpSplashScreen_Closed(object sender, WindowEventArgs args)
        {
            args.Handled = !ViewModel.IsLoaded;
        }

        private void LpSplashScreen_Completed(object sender, Window e)
        {
            Completed -= LpSplashScreen_Completed;
            Closed -= LpSplashScreen_Closed;
            e.Close();
        }

        protected override async Task OnLoading()
        {
            await base.OnLoading();
            await ViewModel.LoadingTask;
        }
    }
}
