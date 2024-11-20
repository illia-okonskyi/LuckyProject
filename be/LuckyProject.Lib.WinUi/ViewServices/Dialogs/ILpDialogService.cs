using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public interface ILpDialogService : IDisposable
    {
        ILpDialogServiceLocalization Localization { get; }
        Task<ContentDialogResult> ShowDialogAsync(LpDialogOptions options);
        Task<LpMessageBoxResult> ShowMessageBoxAsync(LpMessageBoxOptions options);
        /// <summary>
        /// Returns true if finished and false if cancelled
        /// </summary>
        Task<bool> ShowProgressDialogAsync(LpProgressDialogOptions options);
        Task<StorageFolder> PickDirAsync(LpPickerDialogOptions options);
        Task<StorageFile> PickSingleFileAsync(LpOpenFilePickerDialogOptions options);
        Task<List<StorageFile>> PickMultipleFilesAsync(LpOpenFilePickerDialogOptions options);
        Task<StorageFile> PickSaveFileAsync(LpSaveFilePickerDialogOptions options);
    }
}
