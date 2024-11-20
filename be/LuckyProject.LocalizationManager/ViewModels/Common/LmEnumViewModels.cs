using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.LocalizationManager.Models;
using System.Collections.Generic;

namespace LuckyProject.LocalizationManager.ViewModels.Common
{
    public class LmItemStatusEnumValueViewModel : LpEnumValueViewModel<LmProjectDocument.ItemStatus>
    {
        public LmItemStatusEnumValueViewModel(
            LmProjectDocument.ItemStatus? value,
            Dictionary<LmProjectDocument.ItemStatus, string> displayNames = null,
            string nullDisplayName = "Null")
            : base(value, displayNames, nullDisplayName)
        { }
    }

    public class LmResourceTypeEnumValueViewModel : LpEnumValueViewModel<LpLocalizationResourceType>
    {
        public LmResourceTypeEnumValueViewModel(
            LpLocalizationResourceType? value,
            Dictionary<LpLocalizationResourceType, string> displayNames = null,
            string nullDisplayName = "Null")
            : base(value, displayNames, nullDisplayName)
        { }
    }
}
