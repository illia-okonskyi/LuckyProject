using LuckyProject.Lib.Basics.Constants;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;

namespace LuckyProject.Lib.WinUi.Components.Dialogs
{
    public sealed partial class LpContentDialogContent : Page
    {
        public LpContentDialogContent(object dialogContent, string blockTimeLeftHeader)
        {
            InitializeComponent();
            cpDialogContent.Content = dialogContent;
            spBlockContent.Visibility = Visibility.Collapsed;
            rBlockTimeLeftHeader.Text = blockTimeLeftHeader;
        }

        public void SetBlock(bool isBlocked, TimeSpan? blockLeft = null)
        {
            var blockContentVisibility = Visibility.Collapsed;
            if (isBlocked && blockLeft.Value != TimeoutDefaults.Infinity)
            {
                blockContentVisibility = Visibility.Visible;
                rBlockTimeLeft.Text = blockLeft.Value.ToString("g");
            }

            spBlockContent.Visibility = blockContentVisibility;
        }
    }
}
