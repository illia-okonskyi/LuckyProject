using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.WinUi.Components.Controls;
using LuckyProject.Lib.WinUi.Services;
using System;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    #region Localization
    public partial class LpPaginationLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public LpPaginationLocalizationViewModel(ILpDesktopAppLocalizationService service)
            : base(service)
        {
            pagination = service.GetPaginationControlLocalization();
        }

        public override void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            base.OnLocalizationUpdated(service);
            Pagination = service.GetPaginationControlLocalization();
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private PaginationLocalization pagination;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class LpPaginationViewModel : LpViewModel<LpPaginationLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly Action<int> onCurrentPageChanged;

        public LpPaginationViewModel(Action<int> onCurrentPageChanged = null)
        {
            this.onCurrentPageChanged = onCurrentPageChanged;
        }
        #endregion

        #region Public & observables
        [ObservableProperty]
        private PaginationMetadata pagination;
        [ObservableProperty]
        private int currentPage = 1;
        #endregion

        #region Internals
        partial void OnCurrentPageChanged(int value)
        {
            onCurrentPageChanged?.Invoke(value);
        }
        #endregion
    }
    #endregion
}
