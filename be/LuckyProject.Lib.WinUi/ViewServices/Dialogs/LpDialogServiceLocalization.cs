using LuckyProject.Lib.WinUi.Services;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpDialogServiceLocalization
        : ILpDialogServiceLocalization
        , ILpDesktopAppLocalizationConsumer
    {
        #region Internals & ctor & Dispose
        private readonly ILpDesktopAppLocalizationService localizationService;

        public LpDialogServiceLocalization(ILpDesktopAppLocalizationService localizationService)
        {
            this.localizationService = localizationService;
            this.localizationService.AddConsumer(this);
            Update();
        }

        public void Dispose()
        {
            localizationService.DeleteConsumer(this);
        }
        #endregion

        #region Interface impl
        public string Ok { get; private set; }
        public string Yes { get; private set; }
        public string No { get; private set; }
        public string Accept { get; private set; }
        public string Reject { get; private set; }
        public string Cancel { get; private set; }
        public string Close { get; private set; }
        public string PayAttentionFor { get; private set; }
        public string Wait { get; private set; }
        public void OnLocalizationUpdated(ILpDesktopAppLocalizationService service) => Update();
        #endregion

        #region Internals
        private void Update()
        {
            var translations = localizationService.GetLocalizedStrings(new()
            {
                { "lp.lib.winui.common.strings.ok", "OK" },
                { "lp.lib.winui.common.strings.yes", "Yes" },
                { "lp.lib.winui.common.strings.no", "No" },
                { "lp.lib.winui.common.strings.accept", "Accept" },
                { "lp.lib.winui.common.strings.reject", "Reject" },
                { "lp.lib.winui.common.strings.cancel", "Cancel" },
                { "lp.lib.winui.common.strings.close", "Close" },
                { "lp.lib.winui.common.strings.payAttentionFor", "Please, pay attention for" },
                { "lp.lib.winui.common.strings.wait", "Please, wait..." },
            });
            Ok = translations["lp.lib.winui.common.strings.ok"];
            Yes = translations["lp.lib.winui.common.strings.yes"];
            No = translations["lp.lib.winui.common.strings.no"];
            Accept = translations["lp.lib.winui.common.strings.accept"];
            Reject = translations["lp.lib.winui.common.strings.reject"];
            Cancel = translations["lp.lib.winui.common.strings.cancel"];
            Close = translations["lp.lib.winui.common.strings.close"];
            PayAttentionFor = translations["lp.lib.winui.common.strings.payAttentionFor"];
            Wait = translations["lp.lib.winui.common.strings.wait"];
        }
        #endregion

    }
}
