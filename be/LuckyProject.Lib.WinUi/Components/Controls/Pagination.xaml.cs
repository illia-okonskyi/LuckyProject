using LuckyProject.Lib.Basics.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace LuckyProject.Lib.WinUi.Components.Controls
{
    public class PaginationLocalization
    {
        public static readonly PaginationLocalization Default = new()
        {
            ShowingItems = "Showing {0}-{1} of {2} item(s)"
        };

        public string ShowingItems { get; set; }
    }

    public sealed class PaginationPageChangedEventArgs
    {
        public PaginationPageChangedEventArgs(int oldPage, int newPage)
        {
            OldPage = oldPage;
            NewPage = newPage;
        }

        public int OldPage { get; }
        public int NewPage { get; }
    }

    public sealed partial class Pagination : UserControl
    {
        public Pagination()
        {
            InitializeComponent();
            Update(Localization, Metadata);
        }

        #region DependencyProperties
        #region Localization
        public static readonly DependencyProperty LocalizationProperty =
            DependencyProperty.Register(
                nameof(Localization),
                typeof(PaginationLocalization),
                typeof(Pagination),
                new PropertyMetadata(
                    PaginationLocalization.Default,
                    OnLocalizationPropertyChanged));
        public PaginationLocalization Localization
        {
            get => (PaginationLocalization)GetValue(LocalizationProperty);
            set => SetValue(LocalizationProperty, value);
        }

        private static void OnLocalizationPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (Pagination)d;
            var newValue = (PaginationLocalization)e.NewValue;
            self.Update(newValue, self.Metadata);
        }
        #endregion

        #region Metadata
        public static readonly DependencyProperty MetadataProperty =
            DependencyProperty.Register(
                nameof(Metadata),
                typeof(PaginationMetadata),
                typeof(Pagination),
                new PropertyMetadata(null, OnMetadataPropertyChanged));
        public PaginationMetadata Metadata
        {
            get => (PaginationMetadata)GetValue(MetadataProperty);
            set => SetValue(MetadataProperty, value);
        }

        private static void OnMetadataPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (Pagination)d;
            var newValue = (PaginationMetadata)e.NewValue;
            self.Update(self.Localization, newValue);
        }
        #endregion

        #region CurrentPage
        public static readonly DependencyProperty CurrentPageProperty =
            DependencyProperty.Register(
                nameof(CurrentPage),
                typeof(int),
                typeof(Pagination),
                new PropertyMetadata(1, OnCurrentPagePropertyChanged));
        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        private static void OnCurrentPagePropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var self = (Pagination)d;
            self.PageChanged?.Invoke(
                self,
                new PaginationPageChangedEventArgs((int)e.OldValue, (int)e.NewValue));
        }
        #endregion
        #endregion

        #region Events
        public event TypedEventHandler<Pagination, PaginationPageChangedEventArgs> PageChanged;
        #endregion

        #region Internals
        private void Update(PaginationLocalization localization, PaginationMetadata metadata)
        {
            IsEnabled = metadata != null;
            if (!IsEnabled)
            {
                tbShowingItems.Text = string.Format(localization.ShowingItems, 0, 0, 0);
                nbCurrentPage.Maximum = 1;
                CurrentPage = 1;
                tbTotalPagesCount.Text = "1";
                return;
            }

            tbShowingItems.Text = string.Format(
                localization.ShowingItems,
                metadata.FirstPageItem,
                metadata.LastPageItem,
                metadata.TotalItemsCount);
            btnFirstPage.IsEnabled = metadata.Page > 1;
            btnPrevPage.IsEnabled = metadata.PrevPage.HasValue;
            btnNextPage.IsEnabled = metadata.NextPage.HasValue;
            btnLastPage.IsEnabled = metadata.Page < metadata.TotalPagesCount;
            nbCurrentPage.Maximum = metadata.TotalPagesCount;
            tbTotalPagesCount.Text = metadata.TotalPagesCount.ToString();
            CurrentPage = metadata.Page;
        }

        private void btnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
        }

        private void btnPrevPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = Metadata.PrevPage.Value;
        }

        private void btnNextPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = Metadata.NextPage.Value;
        }

        private void btnLastPage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = Metadata.TotalPagesCount;
        }
        #endregion
    }
}
