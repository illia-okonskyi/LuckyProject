using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Common;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;

namespace LuckyProject.LocalizationManager.ViewModels.Pages
{
    #region Localization
    public partial class WorkspacePageLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public WorkspacePageLocalizationViewModel(ILpDesktopAppLocalizationService service)
        : base(service)
        {
            SetProps(GetRegistrations());
            UpdateEnums();
        }

        private List<PropRegistration> GetRegistrations()
        {
            var selfType = GetType();
            return new List<PropRegistration>
                {
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.filter",
                        DefaultValue = "Filter:",
                        Property = selfType.GetProperty(nameof(Filter))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.apply",
                        DefaultValue = "Apply",
                        Property = selfType.GetProperty(nameof(Apply))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.status",
                        DefaultValue = "Status",
                        Property = selfType.GetProperty(nameof(Status))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.type",
                        DefaultValue = "Type",
                        Property = selfType.GetProperty(nameof(Type))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.key",
                        DefaultValue = "Key",
                        Property = selfType.GetProperty(nameof(Key))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.basis",
                        DefaultValue = "Basis",
                        Property = selfType.GetProperty(nameof(Basis))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.translation",
                        DefaultValue = "Translation",
                        Property = selfType.GetProperty(nameof(Translation))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.lib.winui.common.strings.all",
                        DefaultValue = "All",
                        Property = selfType.GetProperty(nameof(All))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.lib.winui.common.strings.any",
                        DefaultValue = "Any",
                        Property = selfType.GetProperty(nameof(Any))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.new",
                        DefaultValue = "New",
                        Property = selfType.GetProperty(nameof(ItemStatusNew))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.setNew",
                        DefaultValue = "Set New (Ctrl + N)",
                        Property = selfType.GetProperty(nameof(ItemStatusSetNew))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.review",
                        DefaultValue = "Review",
                        Property = selfType.GetProperty(nameof(ItemStatusReview))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.setReview",
                        DefaultValue = "Set Review (Ctrl + R)",
                        Property = selfType.GetProperty(nameof(ItemStatusSetReview))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.done",
                        DefaultValue = "Done",
                        Property = selfType.GetProperty(nameof(ItemStatusDone))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemStatuses.setDone",
                        DefaultValue = "Set Done (Ctrl + Enter)",
                        Property = selfType.GetProperty(nameof(ItemStatusSetDone))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemTypes.string",
                        DefaultValue = "String",
                        Property = selfType.GetProperty(nameof(ItemTypeString))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.workspace.strings.itemTypes.file",
                        DefaultValue = "File",
                        Property = selfType.GetProperty(nameof(ItemTypeFile))
                    },
               };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string filter;
        [ObservableProperty]
        private string apply;
        [ObservableProperty]
        private string status;
        [ObservableProperty]
        private string type;
        [ObservableProperty]
        private string key;
        [ObservableProperty]
        private string basis;
        [ObservableProperty]
        private string translation;
        [ObservableProperty]
        private string all;
        [ObservableProperty]
        private string any;
        [ObservableProperty]
        private string itemStatusNew;
        [ObservableProperty]
        private string itemStatusSetNew;
        [ObservableProperty]
        private string itemStatusReview;
        [ObservableProperty]
        private string itemStatusSetReview;
        [ObservableProperty]
        private string itemStatusDone;
        [ObservableProperty]
        private string itemStatusSetDone;
        [ObservableProperty]
        private string itemTypeString;
        [ObservableProperty]
        private string itemTypeFile;
        #endregion

        #region Enums
        public Dictionary<LmProjectDocument.ItemStatus, string> ItemStatuses { get; private set; }
        public Dictionary<LpLocalizationResourceType, string> ItemTypes { get; private set; }
        #endregion

        #region Dependancies registration
        public LpEnumListViewModel<LmProjectDocument.ItemStatus, LmItemStatusEnumValueViewModel>
            FilterStatus
        {
            get;
            set;
        }
        public LpEnumListViewModel<LpLocalizationResourceType, LmResourceTypeEnumValueViewModel>
            FilterResourceType
        {
            get;
            set;
        }
        public LpEnumListViewModel<LmProjectDocument.ItemStatus, LmItemStatusEnumValueViewModel>
            SetStatus
        {
            get;
            set;
        }

        private HashSet<ItemViewModel> items = new();

        public void RegisterItem(ItemViewModel item)
        {
            items.Add(item);
        }

        public void UnregisterItem(ItemViewModel item)
        {
            items.Remove(item);
        }
        #endregion

        #region Internals
        public override void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            base.OnLocalizationUpdated(service);
            UpdateEnums();
        }

        private void UpdateEnums()
        {
            ItemStatuses = new()
            {
                { LmProjectDocument.ItemStatus.New, ItemStatusNew },
                { LmProjectDocument.ItemStatus.Review, ItemStatusReview },
                { LmProjectDocument.ItemStatus.Done, ItemStatusDone },
            };
            ItemTypes = new()
            {
                { LpLocalizationResourceType.String, ItemTypeString },
                { LpLocalizationResourceType.File, ItemTypeFile },
            };

            FilterStatus?.UpdateDisplayNames(ItemStatuses, Any);
            FilterResourceType?.UpdateDisplayNames(ItemTypes, Any);
            SetStatus?.UpdateDisplayNames(ItemStatuses);
            foreach (var item in items)
            {
                item.Status.UpdateDisplayNames(ItemStatuses);
                item.ResourceType.UpdateDisplayNames(ItemTypes);
            }
        }
        #endregion
    }
    #endregion

    #region ItemViewModel
    public partial class ItemViewModel : LpViewModel, IDisposable
    {
        #region ctor & Dispose
        public ItemViewModel(
            LmProjectDocument.Item item,
            WorkspacePageLocalizationViewModel localization,
            ICommand copyKeyCommand,
            ICommand setInProgressCommand,
            ICommand deleteCommand,
            ICommand setNewCommand,
            ICommand setReviewCommand,
            ICommand setDoneCommand)
        {
            this.Localization = localization;
            Status = new(item.Status, this.Localization.ItemStatuses);
            ResourceType = new(item.ResourceType, this.Localization.ItemTypes);
            Key = item.Key;
            Basis = item.Basis;
            transaltion = item.Transaltion;
            if (string.IsNullOrEmpty(transaltion))
            {
                transaltion = Basis;
            }

            isStringResource = item.ResourceType == LpLocalizationResourceType.String;

            CopyKeyCommand = copyKeyCommand;
            SetInProgressCommand = setInProgressCommand;
            DeleteCommand = deleteCommand;
            SetNewCommand = setNewCommand;
            SetReviewCommand = setReviewCommand;
            SetDoneCommand = setDoneCommand;
            this.Localization.RegisterItem(this);
        }

        public void Dispose()
        {
            Localization.UnregisterItem(this);
        }
        #endregion

        #region Public & Observables
        public WorkspacePageLocalizationViewModel Localization { get; }
        public LmItemStatusEnumValueViewModel Status { get; }
        public LmResourceTypeEnumValueViewModel ResourceType { get; }
        public string Key { get; }
        public string Title => Key.Length <= 20 ? Key : $"{Key[0..8]}...{Key[^7..]}";
        public string Basis { get; }
        [ObservableProperty]
        private string transaltion;

        [ObservableProperty]
        private bool isStringResource;


        public ICommand CopyKeyCommand { get; }
        public ICommand SetInProgressCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SetNewCommand { get; }
        public ICommand SetReviewCommand { get; }
        public ICommand SetDoneCommand { get; }
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class WorkspacePageViewModel : LpViewModel<WorkspacePageLocalizationViewModel>
    {
        #region Internals & ctor & Dispose
        private const int ItemsPageSize = 20;
        private static readonly Regex NewlinesBugFixer = new(@"\r\n|\r", RegexOptions.Compiled);

        private readonly ILmDialogService dialogService;
        private readonly ILmProjectService projectService;

        public WorkspacePageViewModel(
            ILmDialogService dialogService,
            ILmProjectService projectService)
        {
            this.dialogService = dialogService;
            this.projectService = projectService;
            this.projectService.CurrentProjectChanged += OnCurrentProjectChanged;
            ItemsPagination = new(OnItemsPageChanged);
            ApplyFilterCommand = new(ApplyFilter);
            CopyItemKeyCommand = new(CopyItemKey);
            SetItemInProgressCommand = new(SetItemInProgress);
            DeleteItemCommand = new(DeleteItem);
            UseBasisCommand = new(UseBasisAsync);
            CreateNewItemCommand = new(CreateNewItemAsync);
            SetFilteredItemsInProgressCommand = new(SetFilteredItemsInProgress);
            SetFilteredItemsStatusCommand = new(SetFilteredItemsStatus);
            SetAllItemsStatusCommand = new(SetAllItemsStatus);
            DeleteFilteredItemsCommand = new(DeleteFilteredItems);
            DeleteAllItemsCommand = new(DeleteAllItems);
            CloseInProgressItemCommand = new(CloseInProgressItem);
            CloseAllInProgressItemsCommand = new(CloseAllInProgressItems);
            SetItemNewCommand = new(SetItemNew);
            SetItemReviewCommand = new(SetItemReview);
            SetItemDoneCommand = new(SetItemDone);
            filterStatus = new(null, null, Localization.ItemStatuses, Localization.Any);
            filterResourceType = new(null, null, Localization.ItemTypes, Localization.Any);
            setStatus = new(
                s => (s.HasValue, (int)s.DefaultIfNull()),
                null,
                Localization.ItemStatuses);
            Localization.FilterStatus = filterStatus;
            Localization.FilterResourceType = filterResourceType;
            Localization.SetStatus = setStatus;
            OnCurrentProjectChanged();
        }

        public override void Dispose()
        {
            projectService.CurrentProjectChanged -= OnCurrentProjectChanged;
            base.Dispose();
        }
        #endregion

        #region Public & observables
        [ObservableProperty]
        private bool isEnabled;

        #region Filter
        [ObservableProperty]
        private LpEnumListViewModel<LmProjectDocument.ItemStatus, LmItemStatusEnumValueViewModel>
            filterStatus;
        [ObservableProperty]
        private LpEnumListViewModel<LpLocalizationResourceType, LmResourceTypeEnumValueViewModel>
            filterResourceType;
        [ObservableProperty]
        private string filterKey;
        public RelayCommand ApplyFilterCommand { get; }
        #endregion

        #region Items
        [ObservableProperty]
        private LpListViewModel<ItemViewModel> items = new();
        [ObservableProperty]
        private ILmProjectService.ItemsOrder itemsOrder = ILmProjectService.ItemsOrder.KeyAsc;
        public LpPaginationViewModel ItemsPagination { get; }
        public RelayCommand<ItemViewModel> CopyItemKeyCommand { get; }
        public RelayCommand<ItemViewModel> SetItemInProgressCommand { get; }
        public RelayCommand<ItemViewModel> DeleteItemCommand { get; }
        #endregion

        #region Actions
        public RelayCommand UseBasisCommand { get; }
        public RelayCommand CreateNewItemCommand { get; }
        public RelayCommand SetFilteredItemsInProgressCommand { get; }
        [ObservableProperty]
        private LpEnumListViewModel<LmProjectDocument.ItemStatus, LmItemStatusEnumValueViewModel>
            setStatus;
        public RelayCommand SetFilteredItemsStatusCommand { get; }
        public RelayCommand SetAllItemsStatusCommand { get; }
        public RelayCommand DeleteFilteredItemsCommand { get; }
        public RelayCommand DeleteAllItemsCommand { get; }
        #endregion

        #region InProgress Items

        [ObservableProperty]
        private LpListViewModel<ItemViewModel> inProgressItems = new();
        public RelayCommand<ItemViewModel> CloseInProgressItemCommand { get; }
        public RelayCommand CloseAllInProgressItemsCommand { get; }
        public RelayCommand<ItemViewModel> SetItemNewCommand { get; }
        public RelayCommand<ItemViewModel> SetItemReviewCommand { get; }
        public RelayCommand<ItemViewModel> SetItemDoneCommand { get; }
        #endregion
        #endregion

        #region Internals
        private void OnCurrentProjectChanged()
        {
            IsEnabled = projectService.CurrentProject != null;
            if (IsEnabled)
            {
                ApplyFilter();
            }
            else
            {
                Items.SelectedValue = null;
                foreach (var item in Items.Values)
                {
                    item.Dispose();
                }
                Items.Values.Clear();
                ItemsPagination.Pagination = null;
            }
        }

        private void ApplyFilter()
        {
            var prevSelectedItem = Items.SelectedValue;
            var items = projectService.GetItems(
                new ILmProjectService.ItemsFilter()
                {
                    Status = FilterStatus.SelectedValue.Value,
                    ResourceType = FilterResourceType.SelectedValue.Value,
                    Key = FilterKey
                },
                ItemsOrder,
                ItemsPagination.CurrentPage,
                ItemsPageSize);
            ItemsPagination.Pagination = items.Pagination;
            foreach (var item in Items.Values)
            {
                item.Dispose();
            }
            Items.Values.Clear();
            items.Items
                .Select(i => new ItemViewModel(
                    i,
                    Localization,
                    CopyItemKeyCommand,
                    SetItemInProgressCommand,
                    DeleteItemCommand,
                    SetItemNewCommand,
                    SetItemReviewCommand,
                    SetItemDoneCommand))
                .ToList()
                .ForEach(Items.Values.Add);
            if (prevSelectedItem != null)
            {
                Items.SelectedValue = Items.Values.FirstOrDefault(
                    i => i.Key.Equals(prevSelectedItem.Key, StringComparison.Ordinal));
            }
        }

        partial void OnItemsOrderChanged(ILmProjectService.ItemsOrder value)
        {
            ApplyFilter();
        }

        private void OnItemsPageChanged(int page)
        {
            ApplyFilter();
        }

        private void SetItemInProgress(ItemViewModel item)
        {
            var inProgressItem = InProgressItems
                .Values
                .FirstOrDefault(i => i.Key.Equals(item.Key, StringComparison.Ordinal));
            if (inProgressItem == null)
            {
                inProgressItem = item;
                InProgressItems.Values.Add(inProgressItem);
            }
            InProgressItems.SelectedValue = inProgressItem;
        }


        private void CopyItemKey(ItemViewModel item)
        {
            var package = new DataPackage();
            package.RequestedOperation = DataPackageOperation.Copy;
            package.SetText(item.Key);
            Clipboard.SetContent(package);
        }

        private void DeleteItem(ItemViewModel item)
        {
            projectService.DeleteItem(item.Key);
            CloseInProgressItem(item);
            ApplyFilter();
        }

        private async void UseBasisAsync()
        {
            if (await dialogService.UseProjectBasisAsync())
            {
                ApplyFilter();
            }
        }

        private async void CreateNewItemAsync()
        {
            var item = await dialogService.CreateProjectItemAsync();
            if (item != null)
            {
                ApplyFilter();
                SetItemInProgress(new ItemViewModel(
                    item,
                    Localization,
                    CopyItemKeyCommand,
                    SetItemInProgressCommand,
                    DeleteItemCommand,
                    SetItemNewCommand,
                    SetItemReviewCommand,
                    SetItemDoneCommand));
            }
        }
        private void SetFilteredItemsInProgress()
        {
            foreach (var item in Items.Values)
            {
                SetItemInProgress(item);
            }
        }
        
        private void SetFilteredItemsStatus()
        {
            projectService.UpdateItems(
                new ILmProjectService.ItemsFilter()
                {
                    Status = FilterStatus.SelectedValue.Value,
                    ResourceType = FilterResourceType.SelectedValue.Value,
                    Key = FilterKey
                },
                ItemsOrder,
                ItemsPagination.CurrentPage,
                ItemsPageSize,
                SetStatus.SelectedValue.Value.Value);
            ApplyFilter();
        }

        private void SetAllItemsStatus()
        {
            projectService.UpdateAllItems(SetStatus.SelectedValue.Value.Value);
            ApplyFilter();
        }

        private void DeleteFilteredItems()
        {
            projectService.DeleteItems(
                new ILmProjectService.ItemsFilter()
                {
                    Status = FilterStatus.SelectedValue.Value,
                    ResourceType = FilterResourceType.SelectedValue.Value,
                    Key = FilterKey
                },
                ItemsOrder,
                ItemsPagination.CurrentPage,
                ItemsPageSize);
            foreach (var item in Items.Values)
            {
                CloseInProgressItem(item);
            }
            ApplyFilter();
        }

        private void DeleteAllItems()
        {
            projectService.DeleteAllItems();
            CloseAllInProgressItems();
            ApplyFilter();
        }

        private void CloseInProgressItem(ItemViewModel item)
        {
            var inProgressItem = InProgressItems
                .Values
                .FirstOrDefault(i => i.Key.Equals(item.Key, StringComparison.Ordinal));
            if (inProgressItem == null)
            {
                return;
            }

            var index = -1;
            if (InProgressItems.SelectedValue == inProgressItem)
            {
                index = Math.Max(0, InProgressItems.Values.IndexOf(inProgressItem) - 1);
            }
            
            InProgressItems.Values.Remove(inProgressItem);
            if (index > 0 && InProgressItems.Values.Count > 0)
            {
                InProgressItems.SelectedValue = InProgressItems.Values[index];
            }
        }

        private void CloseAllInProgressItems()
        {
            InProgressItems.SelectedValue = null;
            InProgressItems.Values.Clear();
        }

        private void SetItemNew(ItemViewModel item)
        {
            projectService.UpdateItem(
                item.Key,
                LmProjectDocument.ItemStatus.New,
                NewlinesBugFixer.Replace(item.Transaltion, "\r\n"));
            item.Status.Value = LmProjectDocument.ItemStatus.New;
            ApplyFilter();
        }

        private void SetItemReview(ItemViewModel item)
        {
            projectService.UpdateItem(
                item.Key,
                LmProjectDocument.ItemStatus.Review,
                NewlinesBugFixer.Replace(item.Transaltion, "\r\n"));
            item.Status.Value = LmProjectDocument.ItemStatus.Review;
            ApplyFilter();
        }

        private void SetItemDone(ItemViewModel item)
        {
            projectService.UpdateItem(
                item.Key,
                LmProjectDocument.ItemStatus.Done,
                NewlinesBugFixer.Replace(item.Transaltion, "\r\n"));
            item.Status.Value = LmProjectDocument.ItemStatus.Done;
            ApplyFilter();
        }
        #endregion
    }
    #endregion
}
