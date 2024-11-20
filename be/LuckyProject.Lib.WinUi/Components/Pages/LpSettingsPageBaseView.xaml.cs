using LuckyProject.Lib.WinUi.ViewModels.Pages;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Pages
{
    public sealed partial class LpSettingsPageBaseView : UserControl
    {
        #region VM & ctor
        public LpSettingsPageBaseViewModel ViewModel { get; set; }

        public LpSettingsPageBaseView()
        {
            InitializeComponent();
        }
        #endregion

        #region DependancyProperties
        #region AppVersion
        public static readonly DependencyProperty AppVersionProperty =
            DependencyProperty.Register(
                nameof(AppVersion),
                typeof(string),
                typeof(LpSettingsPageBaseView),
                new PropertyMetadata(null));
        public string AppVersion
        {
            get => GetValue(AppVersionProperty) as string;
            set => SetValue(AppVersionProperty, value);
        }
        #endregion

        #region AppDescription
        public static readonly DependencyProperty AppDescriptionProperty =
            DependencyProperty.Register(
                nameof(AppDescription),
                typeof(string),
                typeof(LpSettingsPageBaseView),
                new PropertyMetadata(null));
        public string AppDescription
        {
            get => GetValue(AppDescriptionProperty) as string;
            set => SetValue(AppDescriptionProperty, value);
        }
        #endregion

        #region ExtraContent
        public static readonly DependencyProperty ExtraContentProperty =
            DependencyProperty.Register(
                nameof(ExtraContent),
                typeof(object),
                typeof(LpSettingsPageBaseView),
                new PropertyMetadata(null));
        public object ExtraContent
        {
            get => GetValue(ExtraContentProperty);
            set => SetValue(ExtraContentProperty, value);
        }
        #endregion
        #endregion
    }
}
