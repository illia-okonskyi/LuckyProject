using LuckyProject.Lib.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Dialogs
{
    public sealed partial class LpProgressDialogContent : Page
    {
        public string Text { get; }
        public LpProgressViewModel Progress { get; }
        public Visibility ExtraContentSeparatorVisibility { get; }
        public object ExtraContent { get; }

        public LpProgressDialogContent(
            string text,
            LpProgressViewModel progress,
            object extraContent)
        {
            Text = text;
            Progress = progress;
            ExtraContentSeparatorVisibility = extraContent != null
                ? Visibility.Visible
                : Visibility.Collapsed;
            ExtraContent = extraContent;
            InitializeComponent();
        }

        public void SetStatus(string status)
        {
            tbStatus.Text = status;
        }
    }
}
