using LuckyProject.Lib.WinUi.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Controls
{
    public sealed partial class VaildationSummary : UserControl
    {
        public VaildationSummary()
        {
            InitializeComponent();
        }

        #region DependancyProperties
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(
                nameof(ViewModel),
                typeof(LpValidationSummaryViewModel),
                typeof(VaildationSummary),
                new PropertyMetadata(null));
        public LpValidationSummaryViewModel ViewModel
        {
            get => (LpValidationSummaryViewModel)GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }
        #endregion
        #endregion
    }
}
