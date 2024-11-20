using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.LocalizationManager.Views.Dialogs
{
    public sealed partial class CreateUpdateProjectDialog : Page
    {
        public CreateUpdateProjectDialogViewModel ViewModel { get; }
        public CreateUpdateProjectDialog(CreateUpdateProjectDialogViewModel viewModel)
        {
            ViewModel = this.InitViewModel(viewModel);
            InitializeComponent();
        }
    }
}
