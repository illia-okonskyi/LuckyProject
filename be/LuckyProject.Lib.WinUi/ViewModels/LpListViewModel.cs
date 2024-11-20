using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    public partial class LpListViewModel<T> : LpViewModel
    {
        #region Internals & ctors
        private readonly Action<T> onSelectedValueChanged;

        public LpListViewModel(
            List<T> values = null,
            Action<T> onSelectedValueChanged = null)
        {
            if (values != null)
            {
                values.ForEach(this.values.Add);
            }

            this.onSelectedValueChanged = onSelectedValueChanged;
            selectedValue = this.values.FirstOrDefault();
        }

        public LpListViewModel(
            List<T> values,
            T selectedValue,
            Action<T> onSelectedValueChanged = null)
            : this(values, onSelectedValueChanged)
        {
            this.selectedValue = selectedValue;
        }

        public LpListViewModel(
            (List<T> Values, T SelectedValue) setup,
            Action<T> onSelectedValueChanged = null)
            : this(setup.Values, setup.SelectedValue, onSelectedValueChanged)
        { }
        #endregion

        #region Observables
        [ObservableProperty]
        private ObservableCollection<T> values = new();
        [ObservableProperty]
        private T selectedValue;
        partial void OnSelectedValueChanged(T value)
        {
            onSelectedValueChanged?.Invoke(value);
        }
        #endregion
    }
}
