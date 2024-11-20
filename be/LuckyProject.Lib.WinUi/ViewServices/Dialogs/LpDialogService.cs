using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.Components.Dialogs;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.Storage;
using WinUIEx;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpDialogService : ILpDialogService
    {
        #region Internals & ctor & Dispose
        private readonly ILpDesktopAppSettingsService settingsService;
        private readonly ILpDesktopAppLocalizationService localizationService;
        private readonly IThemeSelectorService themeSelectorService;
        private readonly LpDialogServiceLocalization localization;
        private readonly ILpTimerFactory timerFactory;

        public LpDialogService(
            ILpDesktopAppSettingsService settingsService,
            ILpDesktopAppLocalizationService localizationService,
            IThemeSelectorService themeSelectorService,
            ILpTimerFactory timerFactory)
        {
            this.settingsService = settingsService;
            this.localizationService = localizationService;
            this.themeSelectorService = themeSelectorService;
            this.timerFactory = timerFactory;
            localization = new LpDialogServiceLocalization(localizationService);
        }

        public void Dispose()
        {
            localization.Dispose();
        }
        #endregion

        #region Interface impl
        public ILpDialogServiceLocalization Localization => localization;

        public async Task<ContentDialogResult> ShowDialogAsync(LpDialogOptions options)
        {
            var xamlRoot = options.XamlRool
                ?? Application.Current.GetLpApplicationRoot().Shell.XamlRoot;
            options.PrimaryButton ??= new() { Text = Localization.Ok };
            using var dialog = new LpContentDialog(
                timerFactory,
                xamlRoot,
                themeSelectorService.Theme,
                Localization.PayAttentionFor,
                options);
            var lpAppRoot = Application.Current.GetLpApplicationRoot();
            var prevCanClose = lpAppRoot.CanClose;
            lpAppRoot.CanClose = false;
            try
            {
                return await dialog.ShowAsync();
            }
            finally
            {
                lpAppRoot.CanClose = prevCanClose;
            }
        }

        #region ShowMessageBoxAsync
        public async Task<LpMessageBoxResult> ShowMessageBoxAsync(
            LpMessageBoxOptions options)
        {
            var dialogOptions = new LpDialogOptions()
            {
                XamlRool = options.XamlRool,
                Title = options.Title,
                Content = new LpMessageBoxContent(options.Icon, options.Text, options.ExtraContent),
                Block = options.Block,
            };

            ShowMessageBoxAsync_SetupOptions(options, dialogOptions);
            return ShowMessageBoxAsync_MapResult(options, await ShowDialogAsync(dialogOptions));
        }

        private void ShowMessageBoxAsync_SetupOptions(
            LpMessageBoxOptions options,
            LpDialogOptions dialogOptions)
        {
            var buttons = options.Buttons;
            if (options.SingleCustomButton)
            {
                dialogOptions.PrimaryButton = new() { Text = options.CustomDefaultButton };
                dialogOptions.CommandSpaceAlignment = HorizontalAlignment.Center;
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (options.TwoCustomButtons)
            {
                dialogOptions.PrimaryButton = new() { Text = options.CustomDefaultButton };
                dialogOptions.CloseButton = new() { Text = options.CustomCancelButton };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (options.ThreeCustomButtons)
            {
                dialogOptions.PrimaryButton = new() { Text = options.CustomDefaultButton };
                dialogOptions.SecondaryButton = new() { Text = options.CustomSecondaryButton };
                dialogOptions.CloseButton = new() { Text = options.CustomCancelButton };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.Ok)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Ok };
                dialogOptions.CommandSpaceAlignment = HorizontalAlignment.Center;
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.Yes)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Yes };
                dialogOptions.CommandSpaceAlignment = HorizontalAlignment.Center;
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.Accept)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Accept };
                dialogOptions.CommandSpaceAlignment = HorizontalAlignment.Center;
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.Close)
            {
                dialogOptions.CloseButton = new() { Text = Localization.Close };
                dialogOptions.CommandSpaceAlignment = HorizontalAlignment.Center;
                dialogOptions.DefaultButton = ContentDialogButton.Close;
            }
            else if (buttons == LpMessageBoxButtons.YesNo)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Yes };
                dialogOptions.CloseButton = new() { Text = Localization.No };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.YesNoCancel)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Yes };
                dialogOptions.SecondaryButton = new() { Text = Localization.No };
                dialogOptions.CloseButton = new() { Text = Localization.Cancel };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.AcceptReject)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Accept };
                dialogOptions.CloseButton = new() { Text = Localization.Reject };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else if (buttons == LpMessageBoxButtons.AcceptRejectCancel)
            {
                dialogOptions.PrimaryButton = new() { Text = Localization.Accept };
                dialogOptions.SecondaryButton = new() { Text = Localization.Reject };
                dialogOptions.CloseButton = new() { Text = Localization.Cancel };
                dialogOptions.DefaultButton = ContentDialogButton.Primary;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        private LpMessageBoxResult ShowMessageBoxAsync_MapResult(
            LpMessageBoxOptions options,
            ContentDialogResult dialogResult)
        {
            var buttons = options.Buttons;
            if (options.SingleCustomButton)
            {
                return LpMessageBoxResult.CustomPrimary;
            }
            else if (options.TwoCustomButtons)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.CustomPrimary,
                    ContentDialogResult.None => LpMessageBoxResult.CustomCancel,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            else if (options.ThreeCustomButtons)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.CustomPrimary,
                    ContentDialogResult.Secondary => LpMessageBoxResult.CustomSecondary,
                    ContentDialogResult.None => LpMessageBoxResult.CustomCancel,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            if (buttons == LpMessageBoxButtons.Ok)
            {
                return LpMessageBoxResult.Ok;
            }
            else if (buttons == LpMessageBoxButtons.Yes)
            {
                return LpMessageBoxResult.Yes;
            }
            else if (buttons == LpMessageBoxButtons.Accept)
            {
                return LpMessageBoxResult.Accept;
            }
            else if (buttons == LpMessageBoxButtons.Close)
            {
                return LpMessageBoxResult.Close;
            }
            else if (buttons == LpMessageBoxButtons.YesNo)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.Yes,
                    ContentDialogResult.None => LpMessageBoxResult.No,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            else if (buttons == LpMessageBoxButtons.YesNoCancel)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.Yes,
                    ContentDialogResult.Secondary => LpMessageBoxResult.No,
                    ContentDialogResult.None => LpMessageBoxResult.Cancel,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            else if (buttons == LpMessageBoxButtons.AcceptReject)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.Accept,
                    ContentDialogResult.None => LpMessageBoxResult.Reject,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            else if (buttons == LpMessageBoxButtons.AcceptRejectCancel)
            {
                return dialogResult switch
                {
                    ContentDialogResult.Primary => LpMessageBoxResult.Accept,
                    ContentDialogResult.Secondary => LpMessageBoxResult.Reject,
                    ContentDialogResult.None => LpMessageBoxResult.Cancel,
                    _ => throw new InvalidOperationException("Unexpected ContentDialogResult")
                };
            }
            else
            {
                throw new NotImplementedException();
            }
        }
        #endregion

        #region ShowProgressDialogAsync
        public async Task<bool> ShowProgressDialogAsync(LpProgressDialogOptions options)
        {
            var (content, dialog, cancelSource) = ShowProgressDialogAsync_Build(options);
            var appRoot = Application.Current.GetLpApplicationRoot();
            var context = new LpAppTaskContext()
            {
                SetStatus = s => appRoot.TryEnqueueToDispatcherQueue(() => content.SetStatus(s)),
                SetProgressValue = v => appRoot.TryEnqueueToDispatcherQueue(
                    () => content.Progress.Value = v),
                SetProgressIndeterminate = i => appRoot.TryEnqueueToDispatcherQueue(
                    () => content.Progress.IsIndeterminate = i)
            };

            var lpAppRoot = Application.Current.GetLpApplicationRoot();
            var prevCanClose = lpAppRoot.CanClose;
            lpAppRoot.CanClose = false;
            var dialogTask = dialog.ShowAsync();
            try
            {
                await options.Handler(context, cancelSource?.Token ?? default);
                cancelSource?.Dispose();
                dialog.ForceClose();
                await dialogTask;
                return true;
            }
            catch (OperationCanceledException)
            {
                // NOTE: do nothing, swallow and just return
                return false;
            }
            finally
            {
                lpAppRoot.CanClose = prevCanClose;
            }
        }

        private (LpProgressDialogContent, LpContentDialog, CancellationTokenSource)
            ShowProgressDialogAsync_Build(LpProgressDialogOptions options)
        {
            var content = new LpProgressDialogContent(
                options.Text,
                new LpProgressViewModel { IsIndeterminate = options.IsIndeterminate },
                options.ExtraContent);
            content.SetStatus(options.Status);
            var dialogOptions = new LpDialogOptions()
            {
                Title = options.Title,
                Content = content,
                CommandSpaceAlignment = HorizontalAlignment.Center
            };

            CancellationTokenSource cancelSource = null;
            if (options.IsCancelable)
            {
                cancelSource = new CancellationTokenSource();
                Func<object, Task<bool>> cancelHandler = ctx =>
                {
                    cancelSource.Cancel();
                    return Task.FromResult(true);
                };
                dialogOptions.PrimaryButton = new()
                {
                    Text = Localization.Cancel,
                    ClickHandler = cancelHandler
                };
                dialogOptions.CancelHandler = cancelHandler;
            }
            else
            {
                dialogOptions.Block = new()
                {
                    Mode = LpDialogBlockMode.CommandButtonsOnly,
                    Timeout = TimeoutDefaults.Infinity,
                };
                dialogOptions.PrimaryButton = new() { Text = Localization.Wait };
                dialogOptions.ClosePolicy = LpDialogClosePolicy.ButtonsOnly;
            }
            dialogOptions.DefaultButton = ContentDialogButton.Primary;

            var xamlRoot = options.XamlRool
                ?? Application.Current.GetLpApplicationRoot().Shell.XamlRoot;
            using var dialog = new LpContentDialog(
                timerFactory,
                xamlRoot,
                themeSelectorService.Theme,
                Localization.PayAttentionFor,
                dialogOptions);
            if (options.IsCancelable)
            {
                cancelSource.Token.Register(dialog.ForceClose);
            }

            return (content, dialog, cancelSource);
        }
        #endregion

        #region Pickers
        public async Task<StorageFolder> PickDirAsync(LpPickerDialogOptions options)
        {
            var picker = new FolderPicker();
            var hWnd = options.Hwnd
                ?? Application.Current.GetLpApplicationRoot().MainWindow.GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.SettingsIdentifier = options.SettingsIdentifier
                ?? settingsService.PickerDialogsSettingsId;
            picker.ViewMode = options.ViewMode;
            picker.CommitButtonText = options.CommitButtonText;
            if (options.SuggestedStartLocation.HasValue)
            {
                picker.SuggestedStartLocation = options.SuggestedStartLocation.Value;
            }
            return await picker.PickSingleFolderAsync();
        }

        public async Task<StorageFile> PickSingleFileAsync(LpOpenFilePickerDialogOptions options)
        {
            return await CreateFileOpenPicker(options).PickSingleFileAsync();
        }

        public async Task<List<StorageFile>> PickMultipleFilesAsync(
            LpOpenFilePickerDialogOptions options)
        {
            return (await CreateFileOpenPicker(options).PickMultipleFilesAsync()).ToList();
        }

        public async Task<StorageFile> PickSaveFileAsync(LpSaveFilePickerDialogOptions options)
        {
            if (options.FileTypeChoices.IsNullOrEmpty())
            {
                throw new ArgumentException(
                    "Must not be null or empty",
                    nameof(options.FileTypeChoices));
            }

            var picker = new FileSavePicker();
            var hWnd = options.Hwnd
                ?? Application.Current.GetLpApplicationRoot().MainWindow.GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.SettingsIdentifier = options.SettingsIdentifier
                ?? settingsService.PickerDialogsSettingsId;
            picker.CommitButtonText = options.CommitButtonText;
            picker.SuggestedFileName = options.SuggestedFileName;
            picker.SuggestedSaveFile = options.SuggestedSaveFile;
            foreach (var kvp in options.FileTypeChoices)
            {
                picker.FileTypeChoices.Add(kvp.Key, kvp.Value);
            }

            if (options.SuggestedStartLocation.HasValue)
            {
                picker.SuggestedStartLocation = options.SuggestedStartLocation.Value;
            }
            return await picker.PickSaveFileAsync();
        }

        private FileOpenPicker CreateFileOpenPicker(LpOpenFilePickerDialogOptions options)
        {
            var picker = new FileOpenPicker();
            var hWnd = options.Hwnd
                ?? Application.Current.GetLpApplicationRoot().MainWindow.GetWindowHandle();
            WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);

            picker.SettingsIdentifier = options.SettingsIdentifier
                ?? settingsService.PickerDialogsSettingsId;
            picker.ViewMode = options.ViewMode;
            picker.CommitButtonText = options.CommitButtonText;
            options.FileTypeFilter.ForEach(picker.FileTypeFilter.Add);
            if (options.SuggestedStartLocation.HasValue)
            {
                picker.SuggestedStartLocation = options.SuggestedStartLocation.Value;
            }
            return picker;
        }
        #endregion
        #endregion
    }
}
