using CommunityToolkit.WinUI.UI.Controls;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.Helpers;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Pages;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Linq;
using Windows.System;
using Windows.UI.Core;

namespace LuckyProject.LocalizationManager.Views.Pages
{
    public sealed partial class WorkspacePage : Page
    {
        public WorkspacePageViewModel ViewModel { get; }

        public WorkspacePage()
        {
            ViewModel = this.InitViewModel<WorkspacePageViewModel>();
            InitializeComponent();
            Update();
        }

        #region Internals
        private void tbFilterKey_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.ApplyFilterCommand.Execute(null);
            }
        }


        private void Update()
        {
            var sortOrder = ViewModel.ItemsOrder;
            var statusColumn = dgItems.Columns.First(c => c.Tag.ToString() == "Status");
            var typeColumn = dgItems.Columns.First(c => c.Tag.ToString() == "Type");
            var keyColumn = dgItems.Columns.First(c => c.Tag.ToString() == "Key");

            if (sortOrder == ILmProjectService.ItemsOrder.StatusAsc)
            {
                statusColumn.SortDirection = DataGridSortDirection.Descending;
                typeColumn.SortDirection = null;
                keyColumn.SortDirection = null;
            }
            else if (sortOrder == ILmProjectService.ItemsOrder.StatusDesc)
            {
                statusColumn.SortDirection = DataGridSortDirection.Ascending;
                typeColumn.SortDirection = null;
                keyColumn.SortDirection = null;
            }
            else if (sortOrder == ILmProjectService.ItemsOrder.ResourceTypeAsc)
            {
                statusColumn.SortDirection = null;
                typeColumn.SortDirection = DataGridSortDirection.Descending;
                keyColumn.SortDirection = null;
            }
            else if (sortOrder == ILmProjectService.ItemsOrder.ResourceTypeDesc)
            {
                statusColumn.SortDirection = null;
                typeColumn.SortDirection = DataGridSortDirection.Ascending;
                keyColumn.SortDirection = null;
            }
            else if (sortOrder == ILmProjectService.ItemsOrder.KeyAsc)
            {
                statusColumn.SortDirection = null;
                typeColumn.SortDirection = null;
                keyColumn.SortDirection = DataGridSortDirection.Descending;
            }
            else
            {
                statusColumn.SortDirection = null;
                typeColumn.SortDirection = null;
                keyColumn.SortDirection = DataGridSortDirection.Ascending;
            }
        }

        private void dgItems_Sorting(object sender, DataGridColumnEventArgs e)
        {
            var tag = e.Column.Tag.ToString();
            // NOTE: it is little bit strange correlation between sort directions arrows and values
            //       in the DataGrid
            var ascending =
                e.Column.SortDirection == null ||
                e.Column.SortDirection == DataGridSortDirection.Ascending;
            ViewModel.ItemsOrder = (tag, ascending) switch
            {
                ("Status", true) => ILmProjectService.ItemsOrder.StatusAsc,
                ("Status", false) => ILmProjectService.ItemsOrder.StatusDesc,
                ("Type", true) => ILmProjectService.ItemsOrder.ResourceTypeAsc,
                ("Type", false) => ILmProjectService.ItemsOrder.ResourceTypeDesc,
                ("Key", true) => ILmProjectService.ItemsOrder.KeyAsc,
                ("Key", false) => ILmProjectService.ItemsOrder.KeyDesc,
                _ => throw new NotSupportedException()
            };
            Update();
        }

        private void tvInProgress_TabCloseRequested(
            TabView sender,
            TabViewTabCloseRequestedEventArgs args)
        {
            ViewModel.CloseInProgressItemCommand.Execute((ItemViewModel)args.Item);
        }

        private void Grid_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            var ctrlPressed = InputKeyboardSource
                .GetKeyStateForCurrentThread(VirtualKey.Control)
                .HasFlag(CoreVirtualKeyStates.Down);
            var expectedKey = e.Key == VirtualKey.Enter ||
                 e.Key == VirtualKey.R ||
                 e.Key == VirtualKey.N ||
                 e.Key == VirtualKey.F4;
            if (!(ctrlPressed && expectedKey))
            {
                return;
            }

            var item = (ItemViewModel)((Grid)sender).DataContext;
            var setCommand = e.Key switch
            {
                VirtualKey.Enter => ViewModel.SetItemDoneCommand,
                VirtualKey.R => ViewModel.SetItemReviewCommand,
                VirtualKey.N => ViewModel.SetItemNewCommand,
                _ => null
            };
            setCommand?.Execute(item);
            ViewModel.CloseInProgressItemCommand.Execute(item);
            e.Handled = true;
            LpVisualTreeHelper
                .FindVisibleControlByTag<TextBox>(tvInProgress, "Translation")
                ?.Focus(FocusState.Programmatic);
        }
        #endregion
    }
}
