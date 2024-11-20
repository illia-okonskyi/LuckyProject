using System;

namespace LuckyProject.Lib.WinUi.Services
{
    public interface ILpDesktopAppLocalizationConsumer : IDisposable
    {
        void OnLocalizationUpdated(ILpDesktopAppLocalizationService service);
    }
}
