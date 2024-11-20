using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Dialogs
{
    public sealed partial class LpMessageBoxContent : Page
    {
        public Visibility IconVisibility { get; }
        public string IconGlyph { get; }
        public string Text { get; }
        public Visibility ExtraContentSeparatorVisibility { get; }
        public object ExtraContent { get; }

        public LpMessageBoxContent(LpMessageBoxIcon icon, string text, object extraContent)
        {
            IconVisibility = icon != LpMessageBoxIcon.None
                ? Visibility.Visible
                : Visibility.Collapsed;
            IconGlyph = icon switch
            {
                LpMessageBoxIcon.Info => "\uE946",
                LpMessageBoxIcon.Warning => "\uE7BA",
                LpMessageBoxIcon.Error => "\uE783",
                LpMessageBoxIcon.Question => "\uF142",
                _ => null
            };
            Text = text;
            ExtraContentSeparatorVisibility = extraContent != null
                ? Visibility.Visible
                : Visibility.Collapsed;
            ExtraContent = extraContent;
            InitializeComponent();
        }
    }
}
