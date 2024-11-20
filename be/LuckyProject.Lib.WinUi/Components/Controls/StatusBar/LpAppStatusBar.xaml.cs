using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.ViewModels.StatusBar;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace LuckyProject.Lib.WinUi.Components.Controls.StatusBar
{
    public sealed partial class LpAppStatusBar : UserControl
    {
        #region VM & ctor & public props
        public LpAppStatusBarViewModel ViewModel { get; }
        public ObservableCollection<LpAppStatusBarDetailsView> Details { get; } = new();

        public LpAppStatusBar()
        {
            ViewModel = this.InitViewModel<LpAppStatusBarViewModel>();
            InitializeComponent();
            Loaded += LpAppStatusBar_Loaded;
        }

        private void LpAppStatusBar_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= LpAppStatusBar_Loaded;
            UpdateDetails(ExtraDetails);
        }
        #endregion

        #region DependancyProperties
        #region IsExpanded
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(
                nameof(IsExpanded),
                typeof(bool),
                typeof(LpAppStatusBar),
                new PropertyMetadata(false));
        public bool IsExpanded
        {
            get => (bool)GetValue(IsExpandedProperty);
            set => SetValue(IsExpandedProperty, value);
        }

        private static void OnIsExpandedChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (LpAppStatusBar)d;
            var isExpanded = (bool)e.NewValue;
            self.Height = isExpanded ? self.SavedExpandedHeight : self.MinHeight;
        }
        #endregion

        #region SavedExpandedHeight
        public static readonly DependencyProperty SavedExpandedHeightProperty =
            DependencyProperty.Register(
                nameof(SavedExpandedHeight),
                typeof(double),
                typeof(LpAppStatusBar),
                new PropertyMetadata(100.0, OnSavedExpandedHeightChanged));
        public double SavedExpandedHeight
        {
            get => (double)GetValue(SavedExpandedHeightProperty);
            set => SetValue(SavedExpandedHeightProperty, value);
        }

        private static void OnSavedExpandedHeightChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (LpAppStatusBar)d;

            if (self.IsExpanded)
            {
                var savedExpandedHeight = (double)e.NewValue;
                self.Height = savedExpandedHeight;
            }
        }
        #endregion

        #region ExtraHeaderContent
        public static readonly DependencyProperty ExtraHeaderContentProperty =
            DependencyProperty.Register(
                nameof(ExtraHeaderContent),
                typeof(object),
                typeof(LpAppStatusBar),
                new PropertyMetadata(null));
        public object ExtraHeaderContent
        {
            get => GetValue(ExtraHeaderContentProperty);
            set => SetValue(ExtraHeaderContentProperty, value);
        }
        #endregion

        #region ExtraDetails
        public static readonly DependencyProperty ExtraDetailsProperty =
            DependencyProperty.Register(
                nameof(ExtraDetails),
                typeof(IList<LpAppStatusBarDetailsView>),
                typeof(LpAppStatusBar),
                new PropertyMetadata(new List<LpAppStatusBarDetailsView>(), OnExtraDetailsChanged));
        public IList<LpAppStatusBarDetailsView> ExtraDetails
        {
            get => (IList<LpAppStatusBarDetailsView>)GetValue(ExtraDetailsProperty);
            set => SetValue(ExtraDetailsProperty, value);
        }

        private static void OnExtraDetailsChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (LpAppStatusBar)d;
            self.UpdateDetails((IList<LpAppStatusBarDetailsView>)e.NewValue);
        }
        #endregion
        #endregion

        #region Internals
        private void expander_Expanding(Expander sender, ExpanderExpandingEventArgs args)
        {
            Height = SavedExpandedHeight;
        }

        private void expander_Collapsed(Expander sender, ExpanderCollapsedEventArgs args)
        {
            SavedExpandedHeight = Height;
            Height = MinHeight;
        }

        private void UpdateDetails(IList<LpAppStatusBarDetailsView> extraDetails = null)
        {
            Details.Clear();
            Details.Add(new TasksDetailsView(ViewModel));
            Details.Add(new MessagesDetailsView(ViewModel));
            if (extraDetails == null)
            {
                return;
            }

            foreach (var tab in extraDetails)
            {
                Details.Add(tab);
            }
        }
        #endregion
    }
}
