using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.LiveObjects;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Helpers;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading;

namespace LuckyProject.Lib.WinUi.Components.Dialogs
{
    public sealed partial class LpContentDialog : ContentDialog, IDisposable
    {
        #region Internals & ctor & Dispose
        private List<Button> buttons;
        private bool canClose;
        private bool forceClose;
        private bool closingOnCloseButton;

        private readonly ElementTheme theme;
        private readonly HorizontalAlignment commandSpaceAlignment;
        private readonly LpContentDialogContent content;
        private readonly LpDialogOptions options;
        private readonly ILpTimer timer;
        private TimeSpan blockLeftTime;

        public LpContentDialog(
            ILpTimerFactory timerFactory,
            XamlRoot xamlRoot,
            ElementTheme theme,
            string defaultBlockTimeLeftHeader,
            LpDialogOptions options)
        {
            XamlRoot = xamlRoot;
            this.theme = theme;
            commandSpaceAlignment = options.CommandSpaceAlignment;
            Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
            Title = options.Title;
            DefaultButton = options.DefaultButton;
            content = new LpContentDialogContent(
                options.Content,
                options.Block?.CustomBlockTimeLeftHeader ?? defaultBlockTimeLeftHeader);
            Content = content;
            this.options = options;

            if (options.PrimaryButton != null)
            {
                IsPrimaryButtonEnabled = true;
                PrimaryButtonText = options.PrimaryButton.Text;
            }

            if (options.SecondaryButton != null)
            {
                IsSecondaryButtonEnabled = true;
                SecondaryButtonText = options.SecondaryButton.Text;
            }

            if (options.CloseButton != null)
            {
                CloseButtonText = options.CloseButton.Text;
            }

            if (options.Block != null)
            {
                blockLeftTime = options.Block.Timeout;
                if (blockLeftTime != TimeoutDefaults.Infinity)
                {
                    timer = timerFactory.CreateTimer(TimeSpan.FromSeconds(1), true, OnTimer);
                }
            }

            Opened += LpContentDialog_Opened;
            PrimaryButtonClick += LpContentDialog_PrimaryButtonClick;
            SecondaryButtonClick += LpContentDialog_SecondaryButtonClick;
            CloseButtonClick += LpContentDialog_CloseButtonClick;
            Closing += LpContentDialog_Closing;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
        #endregion

        #region Public interface
        public void ForceClose()
        {
            forceClose = true;
            Hide();
        }
        #endregion

        #region Internals
        private void LpContentDialog_Opened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            RequestedTheme = theme;
            var commandSpace = LpVisualTreeHelper.FindControlByName<Grid>(
                sender,
                "CommandSpace");
            buttons = LpVisualTreeHelper.GetDirectChildsOfType<Button>(commandSpace);
            commandSpace.HorizontalAlignment = commandSpaceAlignment;
            UpdateBlocked();
            if (blockLeftTime != default && blockLeftTime != TimeoutDefaults.Infinity)
            {
                timer?.Start();
            }
        }

        private void OnTimer(DateTime dt, CancellationToken ct)
        {
            blockLeftTime -= TimeSpan.FromSeconds(1);
            DispatcherQueue.TryEnqueue(UpdateBlocked);
        }

        private void UpdateBlocked()
        {
            var blockOptions = options.Block;
            var isBlocked = blockOptions != null &&
                (blockLeftTime == TimeoutDefaults.Infinity || blockLeftTime > TimeSpan.Zero);
            if (isBlocked)
            {
                content.SetBlock(true, blockLeftTime);
                if (blockOptions.Mode == LpDialogBlockMode.Dialog)
                {
                    content.IsEnabled = false;
                }
                buttons.ForEach(b => b.IsEnabled = false);

                if (blockLeftTime != TimeoutDefaults.Infinity)
                {
                    timer.Start();
                }
                return;
            }

            content.IsEnabled = true;
            content.SetBlock(false);
            buttons.ForEach(b => b.IsEnabled = true);
        }

        private async void LpContentDialog_PrimaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            var options = this.options.PrimaryButton;
            var accept = true;
            if (options.ClickHandler != null)
            {
                accept = await options.ClickHandler(options.ClickHandlerContext);
            }
            canClose = !accept;
            args.Cancel = !accept;
        }
        private async void LpContentDialog_SecondaryButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            var options = this.options.SecondaryButton;
            var accept = true;
            if (options.ClickHandler != null)
            {
                accept = await options.ClickHandler(options.ClickHandlerContext);
            }
            canClose = !accept;
            args.Cancel = !accept;
        }

        private async void LpContentDialog_CloseButtonClick(
            ContentDialog sender,
            ContentDialogButtonClickEventArgs args)
        {
            var options = this.options.CloseButton;
            var accept = true;
            if (options.ClickHandler != null)
            {
                accept = await options.ClickHandler(options.ClickHandlerContext);
            }
            canClose = !accept;
            if (accept)
            {
                closingOnCloseButton = true;
            }
            else
            {
                args.Cancel = !accept;
            }
        }

        private async void LpContentDialog_Closing(
            ContentDialog sender,
            ContentDialogClosingEventArgs args)
        {
            if (forceClose)
            {
                return;
            }

            if (options.ClosePolicy == LpDialogClosePolicy.ButtonsOnly)
            {
                args.Cancel = !canClose;
                return;
            }
            else if (args.Result == ContentDialogResult.None && !closingOnCloseButton)
            {
                var accept = true;
                if (options.CancelHandler != null)
                {
                    accept = await options.CancelHandler(options.CancelHandlerContext);
                }
                args.Cancel = !accept;
            }

        }
        #endregion
    }
}
