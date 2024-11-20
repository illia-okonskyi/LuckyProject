using LuckyProject.Lib.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Controls
{
    public sealed partial class ValidationError : UserControl
    {
        public ValidationError()
        {
            InitializeComponent();
        }

        #region DependancyProperties
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(LpValidationErrorViewModel),
                typeof(ValidationError),
                new PropertyMetadata(null));
        public LpValidationErrorViewModel ViewModel
        {
            get => (LpValidationErrorViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        #endregion
        #endregion
    }
}
