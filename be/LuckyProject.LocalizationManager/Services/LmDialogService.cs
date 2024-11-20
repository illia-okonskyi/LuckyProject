using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.ViewServices.Dialogs;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using LuckyProject.LocalizationManager.Views.Dialogs;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.Services
{
    public class LmDialogService : ILmDialogService
    {
        #region Internals & ctor
        private readonly ILpDialogService backend;
        private readonly ILmProjectService projectService;

        public LmDialogService(
            ILpDialogService backend,
            ILmProjectService projectService)
        {
            this.backend = backend;
            this.projectService = projectService;
        }
        #endregion

        #region Interface impl
        public async Task<bool> CreateUpdateProjectAsync(CreateUpdateProjectMode mode)
        {
            using var vm = new CreateUpdateProjectDialogViewModel(
                backend,
                projectService,
                mode);
            var r = await backend.ShowDialogAsync(new()
            {
                Title = vm.Title,
                Content = new CreateUpdateProjectDialog(vm),
                PrimaryButton = new()
                {
                    Text = vm.PrimaryButtonText,
                    ClickHandler = vm.PrimaryButtonActionAsync
                },
                CloseButton = new() { Text = backend.Localization.Cancel },
                DefaultButton = ContentDialogButton.Primary,
                CommandSpaceAlignment = HorizontalAlignment.Center
            });
            return r == ContentDialogResult.Primary;
        }

        public async Task<bool> UseProjectBasisAsync(
            CancellationToken cancellationToken = default)
        {
            var file = await backend.PickSingleFileAsync(new()
            {
                FileTypeFilter = LmProjectDocument.BaselineSelectFileTypeFilter
            });
            if (file == null)
            {
                return false;
            }

            return await projectService.UseBasisAsync(file.Path, cancellationToken);
        }

        public async Task<bool> OpenProjectAsync(CancellationToken cancellationToken = default)
        {
            var file = await backend.PickSingleFileAsync(new()
            {
                FileTypeFilter = LmProjectDocument.SelectFileTypeFilter
            });

            if (file == null)
            {
                return false;
            }

            await projectService.OpenProjectAsync(file.Path, cancellationToken);
            return true;
        }

        public async Task<bool> SaveProjectAsAsync(CancellationToken cancellationToken = default)
        {
            var file = await backend.PickSaveFileAsync(new()
            {
                FileTypeChoices = LmProjectDocument.SaveFileTypeChoises,
            });

            if (file == null)
            {
                return false;
            }

            await projectService.SaveProjectAsAsync(file.Path, cancellationToken);
            return true;
        }

        public async Task<bool> CloseProjectAsync(string confirmTitle, string confirmText)
        {
            if (projectService.CurrentProject == null)
            {
                return true;
            }

            if (!Application.Current.GetLpApplicationRoot().HasUnsavedData)
            {
                projectService.CloseProject();
                return true;
            }

            var confirm = await backend.ShowMessageBoxAsync(new()
            {
                Title = confirmTitle,
                Text = confirmText,
                Buttons = LpMessageBoxButtons.YesNoCancel,
            });

            if (confirm == LpMessageBoxResult.Cancel)
            {
                return false;
            }

            if (confirm == LpMessageBoxResult.Yes)
            {
                await projectService.SaveProjectAsync();
            }

            projectService.CloseProject();
            return true;
        }

        public async Task<LmProjectDocument.Item> CreateProjectItemAsync()
        {
            using var vm = new CreateProjectItemDialogViewModel(projectService);
            var r = await backend.ShowDialogAsync(new()
            {
                Title = vm.Title,
                Content = new CreateProjectItemDialog(vm),
                PrimaryButton = new()
                {
                    Text = vm.PrimaryButtonText,
                    ClickHandler = vm.PrimaryButtonActionAsync
                },
                CloseButton = new() { Text = backend.Localization.Cancel },
                DefaultButton = ContentDialogButton.Primary,
                CommandSpaceAlignment = HorizontalAlignment.Center
            });
            return r == ContentDialogResult.Primary ? vm.CreatedItem : null;
        }
        #endregion
    }
}
