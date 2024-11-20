using LuckyProject.Lib.WinUi.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Activation
{
    public class ActivationService<TSettings> : IActivationService
        where TSettings : class, new()
    {
        #region Internals & ctor
        private readonly IEnumerable<IActivationHandler> handlers;
        private readonly ILpDesktopAppSettingsService<TSettings> settingsService;
        private readonly IThemeSelectorService themeSelectorService;

        public ActivationService(
            IEnumerable<IActivationHandler> handlers,
            ILpDesktopAppSettingsService<TSettings> settingsService,
            IThemeSelectorService themeSelectorService)
        {
            this.handlers = handlers;
            this.settingsService = settingsService;
            this.themeSelectorService = themeSelectorService;
        }
        #endregion

        #region Public interface
        public async Task ActivateAsync(
            object activationArgs,
            Action shellPageSetter,
            Action mainWindowActivator)
        {
            shellPageSetter();
            await HandleActivationAsync(activationArgs);
            mainWindowActivator();
            themeSelectorService.SetTheme(settingsService.CurrentTheme);
        }
        #endregion

        #region Internals
        private async Task HandleActivationAsync(object activationArgs)
        {
            var handler = handlers.FirstOrDefault(h => h.CanHandle(activationArgs));
            if (handler != null)
            {
                await handler.HandleAsync(activationArgs);
            }
        }
        #endregion
    }
}