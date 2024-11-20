using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.Services;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Threading.Tasks;
using WinUIEx;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public class LpMainWindow : WindowEx
    {
        #region Internals & ctor
        private readonly Func<Task<bool>> onClosingAsync;
        private bool forceClose;

        public LpMainWindow(Func<Task<bool>> onClosingAsync)
        {
            this.onClosingAsync = onClosingAsync;

            SystemBackdrop = new MicaBackdrop();
            Closed += LpMainWindow_Closed;
        }
        #endregion

        #region Public interface
        public void Activate(ILpDesktopAppSettingsService settingsService)
        {
            var settings = settingsService.MainWindowStatePositionSize;
            AppWindow.Resize(new() { Width = settings.Width, Height = settings.Height });
            if (settings.X.HasValue && settings.Y.HasValue)
            {
                AppWindow.Move(new() { X = settings.X.Value, Y = settings.Y.Value });
            }
            else
            {
                this.CenterOnScreen();
            }

            if (settings.State == LpWindowState.Normal)
            {
                Activate();
                return;
            }

            if (settings.State == LpWindowState.Maximized)
            {
                this.Maximize();
                Activate();
                return;
            }
            
            // NOTE: Minimized
            this.Minimize();
        }

        public void ForceClose()
        {
            forceClose = true;
            Close();
        }
        #endregion

        #region Internals
        private async void LpMainWindow_Closed(object sender, WindowEventArgs args)
        {
            if (forceClose)
            {
                Closed -= LpMainWindow_Closed;
                return;
            }

            args.Handled = true;
            forceClose = await onClosingAsync();
            if (forceClose)
            {
                Close();
            }
        }
        #endregion
    }
}