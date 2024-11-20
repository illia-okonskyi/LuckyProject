using LuckyProject.Lib.WinUi.ViewModels.StatusBar;

namespace LuckyProject.Lib.WinUi.Components.Controls.StatusBar
{
    public sealed partial class MessagesDetailsView : LpAppStatusBarDetailsView
    {
        public LpAppStatusBarViewModel ViewModel { get; }

        public MessagesDetailsView(LpAppStatusBarViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
