using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    #region LpEnumValueViewModel
    public partial class LpEnumValueViewModel<TEnum> : LpViewModel
        where TEnum : struct, Enum
    {
        #region Internals & ctor
        private Dictionary<TEnum, string> displayNames;
        private string nullDisplayName;

        public LpEnumValueViewModel(
            TEnum? value,
            Dictionary<TEnum, string> displayNames = null,
            string nullDisplayName = DefaultNullDisplayName)
        {
            this.displayNames = displayNames ?? BuildDefaultDisplayNames();
            this.nullDisplayName = nullDisplayName;
            this.value = value;
        }
        #endregion

        #region Observables
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(DisplayName))]
        private TEnum? value;
        public string DisplayName => Value.HasValue ? displayNames[Value.Value] : nullDisplayName;
        #endregion

        #region UpdateDisplayNames
        public void UpdateDisplayNames(
            Dictionary<TEnum, string> displayNames,
            string nullDisplayName = DefaultNullDisplayName)
        {
            this.displayNames = displayNames;
            this.nullDisplayName = nullDisplayName;
            OnPropertyChanged(nameof(DisplayName));
        }
        #endregion

        #region Helpers
        public const string DefaultNullDisplayName = "Null";
        public static Dictionary<TEnum, string> BuildDefaultDisplayNames()
        {
            return Enum
                .GetValues(typeof(TEnum))
                .Cast<TEnum>()
                .ToDictionary(e => e, e => e.ToString());
        }
        #endregion
    }
    #endregion

    #region LpEnumListViewModel
    public partial class LpEnumListViewModel<TEnum, TValueViewModel>
        : LpListViewModel<TValueViewModel>
        where TEnum : struct, Enum
        where TValueViewModel : LpEnumValueViewModel<TEnum>
    {
        #region ctor
        public LpEnumListViewModel(
            Func<TEnum?, (bool Include, int Index)> filter = null,
            TEnum? selectedValue = default,
            Dictionary<TEnum, string> displayNames = null,
            string nullDisplayName = LpEnumValueViewModel<TEnum>.DefaultNullDisplayName,
            Action<TValueViewModel> onSelectedValueChanged = null)
            : base(
                  BuildSetup(
                      filter,
                      selectedValue,
                      displayNames,
                      nullDisplayName),
                  onSelectedValueChanged)
        { }
        #endregion

        #region UpdateDisplayNames
        public void UpdateDisplayNames(
            Dictionary<TEnum, string> displayNames,
            string nullDisplayName = LpEnumValueViewModel<TEnum>.DefaultNullDisplayName)
        {
            foreach (var value in Values)
            {
                value.UpdateDisplayNames(displayNames, nullDisplayName);
            }
        }
        #endregion

        #region Helpers
        public static (List<TValueViewModel> Values, TValueViewModel SelectedValue) BuildSetup(
            Func<TEnum?, (bool Include, int Index)> filter = null,
            TEnum? selectedValue = default,
            Dictionary<TEnum, string> displayNames = null,
            string nullDisplayName = LpEnumValueViewModel<TEnum>.DefaultNullDisplayName)
        {
            displayNames ??= LpEnumValueViewModel<TEnum>.BuildDefaultDisplayNames();
            var allEnumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum?>().Prepend(null).ToList();
            var valueVms = allEnumValues.Select(
                (e, i) =>
                {
                    var f = filter?.Invoke(e);
                    var include = f?.Include ?? true;
                    var index = f?.Index ?? i;
                    return new { Value = e, Include = include, Index = index };
                })
                .Where(e => e.Include)
                .OrderBy(v => v.Index)
                .Select(v => (TValueViewModel)Activator.CreateInstance(
                    typeof(TValueViewModel),
                    v.Value,
                    displayNames,
                    nullDisplayName))
                .ToList();
            var selectedValueVm = valueVms.FirstOrDefault(
                v => EqualityComparer<TEnum?>.Default.Equals(v.Value, selectedValue));
            selectedValueVm ??= valueVms.FirstOrDefault();
            return (valueVms, selectedValueVm);
        }
        #endregion
    }
    #endregion
}
