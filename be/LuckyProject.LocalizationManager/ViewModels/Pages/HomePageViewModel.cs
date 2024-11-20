using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.Lib.WinUi.ViewModels;
using LuckyProject.LocalizationManager.Services;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using Microsoft.UI.Xaml;
using System.Collections.Generic;

namespace LuckyProject.LocalizationManager.ViewModels.Pages
{
    #region Localization
    public partial class HomePageLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public HomePageLocalizationViewModel(ILpDesktopAppLocalizationService service)
        : base(service)
        {
            SetProps(GetRegistrations());
        }

        private List<PropRegistration> GetRegistrations()
        {
            var selfType = GetType();
            return new List<PropRegistration>
                {
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.newProject",
                        DefaultValue = "New Project",
                        Property = selfType.GetProperty(nameof(NewProject))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.openProject",
                        DefaultValue = "Open Project",
                        Property = selfType.GetProperty(nameof(OpenProject))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.saveProject",
                        DefaultValue = "Save Project",
                        Property = selfType.GetProperty(nameof(SaveProject))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.saveProjectAs",
                        DefaultValue = "Save Project As...",
                        Property = selfType.GetProperty(nameof(SaveProjectAs))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.closeProject",
                        DefaultValue = "Close Project",
                        Property = selfType.GetProperty(nameof(CloseProject))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.lib.winui.entrypoint.strings.closeConfirmation",
                        DefaultValue = "Close confirmation",
                        Property = selfType.GetProperty(nameof(CloseProjectConfirmationTitle))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.unsavedProjectQuestion",
                        DefaultValue =
                            "There are unsaved data in the current project.r\r\nDo you want to " +
                            "save the project before closing it?",
                        Property = selfType.GetProperty(nameof(CloseProjectConfirmationText))
                    },
                    new()
                    {
                        Type = LpLocalizationResourceType.String,
                        Key = "lp.app.lm.pages.home.strings.buildProject",
                        DefaultValue = "Exit",
                        Property = selfType.GetProperty(nameof(BuildProject))
                    },
               };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string newProject;
        [ObservableProperty]
        private string openProject;
        [ObservableProperty]
        private string saveProject;
        [ObservableProperty]
        private string saveProjectAs;
        [ObservableProperty]
        private string closeProject;
        [ObservableProperty]
        private string closeProjectConfirmationTitle;
        [ObservableProperty]
        private string closeProjectConfirmationText;
        [ObservableProperty]
        private string buildProject;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class HomePageViewModel : LpViewModel<HomePageLocalizationViewModel>
    {
        #region Internals & ctor
        private readonly ILmDialogService dialogService;
        private readonly ILmProjectService projectService;

        public HomePageViewModel(
            ILmDialogService dialogService,
            ILmProjectService projectService)
        {
            this.dialogService = dialogService;
            this.projectService = projectService;
            NewProjectCommand = new RelayCommand(NewProjectAsync);
            OpenProjectCommand = new RelayCommand(OpenProjectAsync);
            SaveProjectCommand = new RelayCommand(
                SaveProjectAsync,
                () => projectService.CurrentProject != null);
            SaveProjectAsCommand = new RelayCommand(
                SaveProjectAsAsync,
                () => projectService.CurrentProject != null);
            BuildProjectCommand = new RelayCommand(
                BuildProjectAsync,
                () => projectService.CurrentProject != null);
            CloseProjectCommand = new RelayCommand(
                CloseProjectAsync,
                () => projectService.CurrentProject != null);
            UpdateProjectCommand = new RelayCommand(
                UpdateProjectAsync,
                () => projectService.CurrentProject != null);
            OnCurrentProjectUpdated();
        }
        #endregion

        #region Public & observables
        [ObservableProperty]
        private string currentProjectName;

        public RelayCommand NewProjectCommand { get; }
        public RelayCommand OpenProjectCommand { get; }
        public RelayCommand SaveProjectCommand { get; }
        public RelayCommand SaveProjectAsCommand { get; }
        public RelayCommand BuildProjectCommand { get; }
        public RelayCommand CloseProjectCommand { get; }
        public RelayCommand UpdateProjectCommand { get; }
        #endregion

        #region Internals
        private async void NewProjectAsync()
        {
            if (!await dialogService.CloseProjectAsync(
                Localization.CloseProjectConfirmationTitle,
                Localization.CloseProjectConfirmationText))
            {
                return;
            }

            OnCurrentProjectUpdated();
            if (await dialogService.CreateUpdateProjectAsync(
                CreateUpdateProjectMode.Create))
            {
                OnCurrentProjectUpdated();
            }
        }

        private async void OpenProjectAsync()
        {
            if (!await dialogService.CloseProjectAsync(
                Localization.CloseProjectConfirmationTitle,
                Localization.CloseProjectConfirmationText))
            {
                return;
            }

            OnCurrentProjectUpdated();
            if (await dialogService.OpenProjectAsync())
            {
                OnCurrentProjectUpdated();
            }
        }

        private async void SaveProjectAsync()
        {
            await projectService.SaveProjectAsync();
        }

        private async void SaveProjectAsAsync()
        {
            await dialogService.SaveProjectAsAsync();
        }

        private async void BuildProjectAsync()
        {
            await projectService.BuildProjectAsync();
        }

        private async void CloseProjectAsync()
        {
            if (await dialogService.CloseProjectAsync(
                Localization.CloseProjectConfirmationTitle,
                Localization.CloseProjectConfirmationText))
            {
                OnCurrentProjectUpdated();
            }
        }

        private async void UpdateProjectAsync()
        {
            if (await dialogService.CreateUpdateProjectAsync(CreateUpdateProjectMode.Update))
            {
                OnCurrentProjectUpdated();
            }
        }

        private void OnCurrentProjectUpdated()
        {
            CurrentProjectName = projectService.CurrentProject?.Name;
            SaveProjectCommand.NotifyCanExecuteChanged();
            SaveProjectAsCommand.NotifyCanExecuteChanged();
            BuildProjectCommand.NotifyCanExecuteChanged();
            CloseProjectCommand.NotifyCanExecuteChanged();
            UpdateProjectCommand.NotifyCanExecuteChanged();
        }
        #endregion
    }
    #endregion
}

