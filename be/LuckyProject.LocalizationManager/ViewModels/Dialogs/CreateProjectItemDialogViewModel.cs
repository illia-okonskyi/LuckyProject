using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.ViewModels.Dialogs
{
    #region Localization
    public partial class CreateProjectItemDialogLocalizationViewModel
        : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public CreateProjectItemDialogLocalizationViewModel(
            ILpDesktopAppLocalizationService service)
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
                        Key = "lp.app.lm.pages.workspace.createItemDialog.title",
                        DefaultValue = "Create New Item...",
                        Property = selfType.GetProperty(nameof(Title))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.actions.create",
                        DefaultValue = "Create",
                        Property = selfType.GetProperty(nameof(Create))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.workspace.createItemDialog.type",
                        DefaultValue = "Type:",
                        Property = selfType.GetProperty(nameof(Type))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.workspace.createItemDialog.key",
                        DefaultValue = "Key:",
                        Property = selfType.GetProperty(nameof(Key))
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
        private string title;
        [ObservableProperty]
        private string create;
        [ObservableProperty]
        private string type;
        [ObservableProperty]
        private string key;
        [ObservableProperty]
        private string itemTypeString;
        [ObservableProperty]
        private string itemTypeFile;
        #endregion

        #region Enums
        public Dictionary<LpLocalizationResourceType, string> ItemTypes { get; private set; }
        #endregion

        #region Dependancies registration
        public LpEnumListViewModel<LpLocalizationResourceType, LmResourceTypeEnumValueViewModel>
            ItemType
        {
            get;
            set;
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
            ItemTypes = new()
            {
                { LpLocalizationResourceType.String, ItemTypeString },
                { LpLocalizationResourceType.File, ItemTypeFile },
            };

            ItemType?.UpdateDisplayNames(ItemTypes);
        }
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class CreateProjectItemDialogViewModel
        : LpFormViewModel<CreateProjectItemDialogLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly ILmProjectService projectService;

        public CreateProjectItemDialogViewModel(ILmProjectService projectService)
        {
            this.projectService = projectService;
            Title = Localization.Title;
            PrimaryButtonText = Localization.Create;
            ItemType = new(
                rt => (rt.HasValue, (int)rt.DefaultIfNull()),
                null,
                Localization.ItemTypes);
            Localization.ItemType = ItemType;
            RegisterValidationFields();
        }
        #endregion

        #region Public & observables
        public string Title { get; }
        public string PrimaryButtonText { get; }
        public LpEnumListViewModel<LpLocalizationResourceType, LmResourceTypeEnumValueViewModel>
            ItemType { get; }
        [ObservableProperty]
        private string key;

        public LmProjectDocument.Item CreatedItem { get; private set; }
        #endregion

        #region Actions
        public Task<bool> PrimaryButtonActionAsync(object ctx)
        {
            return Task.FromResult(ValidationGuard(() =>
            {
                CreatedItem = projectService.CreateItem(new()
                {
                    ResourceType = ItemType.SelectedValue.Value.Value,
                    Key = Key
                });
            }));
        }
        #endregion

        #region Internals
        private void RegisterValidationFields()
        {
            var fields = new List<LpValidationViewModel.FieldRegistration>
            {
                new() { ValidationPath = "Key", DisplayName = "Key" },
            };
            Validation.RegisterFields(fields);
        }
        #endregion
    }
    #endregion
}
