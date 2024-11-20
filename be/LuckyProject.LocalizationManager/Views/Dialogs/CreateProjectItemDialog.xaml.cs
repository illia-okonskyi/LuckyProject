using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.LocalizationManager.Views.Dialogs
{
    public sealed partial class CreateProjectItemDialog : Page
    {
        public CreateProjectItemDialogViewModel ViewModel { get; }
        public CreateProjectItemDialog(CreateProjectItemDialogViewModel viewModel)
        {
            ViewModel = this.InitViewModel(viewModel);
            InitializeComponent();
        }
    }
}
