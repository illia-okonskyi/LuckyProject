using LuckyProject.Lib.WinUi.ViewModels.StatusBar;

namespace LuckyProject.Lib.WinUi.Components.Controls.StatusBar
{
    public sealed partial class TasksDetailsView : LpAppStatusBarDetailsView
    {
        public LpAppStatusBarViewModel ViewModel { get; }

        public TasksDetailsView(LpAppStatusBarViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }
    }
}
