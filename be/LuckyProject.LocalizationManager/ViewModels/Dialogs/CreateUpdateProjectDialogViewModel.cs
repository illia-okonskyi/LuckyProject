using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Components.Controls;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project;
using Microsoft.UI.Xaml;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.ViewModels.Dialogs
{
    #region Definitions
    public enum CreateUpdateProjectMode
    {
        Create,
        Update
    }
    #endregion

    #region Localization
    public partial class CreateUpdateProjectDialogLocalizationViewModel
        : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public CreateUpdateProjectDialogLocalizationViewModel(ILpDesktopAppLocalizationService service)
        : base(service)
        {
            SetProps(GetRegistrations());
            FsPicker = service.GetFsPickerControlLocalization();
        }

        private List<PropRegistration> GetRegistrations()
        {
            var selfType = GetType();
            return new List<PropRegistration>
                {
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.dialogs.createUpdateDialog.strings.newProject",
                        DefaultValue = "New Project...",
                        Property = selfType.GetProperty(nameof(NewProject))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.dialogs.createUpdateDialog.strings.editProject",
                        DefaultValue = "Edit Project",
                        Property = selfType.GetProperty(nameof(EditProject))
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
                        Key = "lp.app.lm.common.strings.actions.update",
                        DefaultValue = "Update",
                        Property = selfType.GetProperty(nameof(Update))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.captions.name",
                        DefaultValue = "Name:",
                        Property = selfType.GetProperty(nameof(Name))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.captions.description",
                        DefaultValue = "Description:",
                        Property = selfType.GetProperty(nameof(Description))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.captions.source",
                        DefaultValue = "Source:",
                        Property = selfType.GetProperty(nameof(Source))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.captions.locale",
                        DefaultValue = "Locale:",
                        Property = selfType.GetProperty(nameof(Locale))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.common.strings.captions.file",
                        DefaultValue = "File:",
                        Property = selfType.GetProperty(nameof(File))
                    },
               };
        }

        public override void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            base.OnLocalizationUpdated(service);
            FsPicker = service.GetFsPickerControlLocalization();
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string newProject;
        [ObservableProperty]
        private string editProject;
        [ObservableProperty]
        private string create;
        [ObservableProperty]
        private string update;
        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string description;
        [ObservableProperty]
        private string source;
        [ObservableProperty]
        private string locale;
        [ObservableProperty]
        private string file;

        [ObservableProperty]
        private FsPickerLocalization fsPicker;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class CreateUpdateProjectDialogViewModel
        : LpFormViewModel<CreateUpdateProjectDialogLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly ILmProjectService projectService;
        private readonly CreateUpdateProjectMode mode;

        public CreateUpdateProjectDialogViewModel(
            ILpDialogService dialogService,
            ILmProjectService projectService,
            CreateUpdateProjectMode mode)
        {
            RegisterValidationFields();

            this.projectService = projectService;
            this.mode = mode;
            DialogService = dialogService;
            DefaultLocales = Localization.Service.GetDefaultLocales();
            if (mode == CreateUpdateProjectMode.Create)
            {
                Title = Localization.NewProject;
                PrimaryButtonText = Localization.Create;
                FileFieldVisibility = Visibility.Visible;
                IsDefaultLocale = true;
                SelectedDefaultLocale = DefaultLocales
                    .First(l => l.Name.Equals("en-us", System.StringComparison.OrdinalIgnoreCase));
            }
            else
            {
                Title = Localization.EditProject;
                PrimaryButtonText = Localization.Update;
                FileFieldVisibility = Visibility.Collapsed;
                var project = projectService.CurrentProject;
                Name = project?.Name;
                Description = project?.Description;
                Source = project?.Source;

                var projectDefaultLocale = DefaultLocales
                    .FirstOrDefault(l => l.Name.Equals(
                        project?.Locale,
                        System.StringComparison.OrdinalIgnoreCase));
                if (projectDefaultLocale != null)
                {
                    IsDefaultLocale = true;
                    SelectedDefaultLocale = projectDefaultLocale;
                    CustomLocale = null;
                }
                else
                {
                    CustomLocale = project?.Locale;
                    IsCustomLocale = !string.IsNullOrEmpty(CustomLocale);
                    IsDefaultLocale = !IsCustomLocale;
                    SelectedDefaultLocale = DefaultLocales
                        .First(l => l.Name.Equals("en-us", System.StringComparison.OrdinalIgnoreCase));
                }
            }

        }
        #endregion

        #region Public & observables
        public string Title { get; }
        public string PrimaryButtonText { get; }
        public Visibility FileFieldVisibility { get; }

        [ObservableProperty]
        private string name;
        [ObservableProperty]
        private string description;
        [ObservableProperty]
        private string source;
        [ObservableProperty]
        private bool isDefaultLocale;
        [ObservableProperty]
        private bool isCustomLocale;
        public List<LpLocaleInfo> DefaultLocales { get; }
        [ObservableProperty]
        private LpLocaleInfo selectedDefaultLocale;
        [ObservableProperty]
        private string customLocale;
        [ObservableProperty]
        private string filePath;

        public ILpDialogService DialogService { get; }
        public FsPickerOptions FsPickerOptions { get; } = new()
        {
            SaveFileTypeChoices = LmProjectDocument.SaveFileTypeChoises
        };
        #endregion

        #region Actions
        public async Task<bool> PrimaryButtonActionAsync(object ctx)
        {
            return await ValidationGuardAsync(async () =>
            {
                if (mode == CreateUpdateProjectMode.Create)
                {
                    await projectService.CreateProjectAsync(new()
                    {
                        Name = Name,
                        Description = Description,
                        Source = Source,
                        Locale = IsDefaultLocale ? SelectedDefaultLocale.Name : CustomLocale,
                        FilePath = FilePath
                    });
                    return;
                }

                projectService.UpdateProject(new()
                {
                    Name = Name,
                    Description = Description,
                    Source = Source,
                    Locale = IsDefaultLocale ? SelectedDefaultLocale.Name : CustomLocale,
                });
            });
        }
        #endregion

        #region Internals
        private void RegisterValidationFields()
        {
            var fields = new List<LpValidationViewModel.FieldRegistration>
            {
                new() { ValidationPath = "Name", DisplayName = "Name" },
                new() { ValidationPath = "Description", DisplayName = "Description" },
                new() { ValidationPath = "Source", DisplayName = "Source" },
                new() { ValidationPath = "Locale", DisplayName = "Locale" },
                new() { ValidationPath = "FilePath", DisplayName = "File" },
            };
            Validation.RegisterFields(fields);
        }
        #endregion
    }
    #endregion
}
