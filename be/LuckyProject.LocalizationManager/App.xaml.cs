using LuckyProject.Lib.WinUi.EntryPoint;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.ViewServices;
using LuckyProject.Lib.WinUi.ViewServices.Navigation;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project;
using LuckyProject.LocalizationManager.ViewModels.Pages;
using LuckyProject.LocalizationManager.Views.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WinUIEx;

namespace LuckyProject.LocalizationManager
{
    public partial class App : Application, ILpApplicationRootProvider
    {
        #region ctor & Internals

        private const string AppName = "Lucky Project Localization Manager";

        public App()
        {
            InitializeComponent();
        }
        #endregion

        #region ILpApplicationProvider
        public ILpApplicationRoot LpApplicationRoot { get; private set; }
        #endregion

        #region Internals
        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var lpApplicationRoot = new LpApplicationRoot<LmSettings>(new()
            {
                SaveAppDataAsyncHandler = SaveAppDataAsync
            });
            LpApplicationRoot = lpApplicationRoot;
            await lpApplicationRoot.OnLaunched(
                args,
                new()
                {
                    ImageUri = new(
                        Path.Combine(Directory.GetCurrentDirectory(),
                            "Resources/Images/SplashScreen.png")),
                    AppName = AppName
                },
                InitAsync,
                mw =>
                {
                    mw.Title = AppName;
                    mw.SetIcon("Resources/Images/Icon.ico");
                    mw.MinWidth = 800;
                    mw.MinHeight = 600;
                });
            base.OnLaunched(args);
        }

        private async Task<List<INavigationService.NavItemRegistration>> InitAsync(
            LpApplicationInitContext<LmSettings> context)
        {
            await context.LoadLocalizationSourceAsync("lp-app-lm");
            return BuildNavItems();
        }

        private List<INavigationService.NavItemRegistration> BuildNavItems()
        {
            return new List<INavigationService.NavItemRegistration>()
            {
                new()
                {
                    TitleLocalizationKey = "lp.app.lm.pages.settings.strings.title",
                    DefaultTitle = "Settings",
                    Page = new()
                    {
                        Type = IPagesService.PageType.Settings,
                        ViewModelType = typeof(SettingsPageViewModel),
                        ViewType = typeof(SettingsPage),
                    }
                },
                new()
                {
                    TitleLocalizationKey = "lp.app.lm.pages.home.strings.title",
                    DefaultTitle = "Home",
                    Icon = new FontIcon { Glyph = "\uE80F" },
                    Page = new()
                    {
                        Type = IPagesService.PageType.Starting,
                        ViewModelType = typeof(HomePageViewModel),
                        ViewType = typeof(HomePage),
                    }
                },
                new()
                {
                    TitleLocalizationKey = "lp.app.lm.pages.workspace.strings.title",
                    DefaultTitle = "Workspace",
                    Icon = new FontIcon { Glyph = "\uE821" },
                    Page = new()
                    {
                        ViewModelType = typeof(WorkspacePageViewModel),
                        ViewType = typeof(WorkspacePage),
                    }
                }
            };
        }

        private async Task SaveAppDataAsync(
            LpAppTaskContext ctx,
            CancellationToken cancellationToken)
        {
            ctx.SetProgressIndeterminate(true);
            await LpApplicationRoot
                .ServiceProvider
                .GetRequiredService<ILmProjectService>()
                .SaveProjectAsync(cancellationToken);
        }
        #endregion
    }
}
