using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project.Requests;
using LuckyProject.LocalizationManager.Services.Project.Validators;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.Services.Project
{
    #region Localization
    public class LmProjectServiceLocalization
    {
        private static readonly Dictionary<string, string> KeysAndDefaults = new()
        {
            { "lp.app.lm.services.project.strings.category", "Project" },
            {
                "lp.app.lm.services.project.strings.messages.createdProject",
                "Project {0} created at location {1}"
            },
            {
                "lp.app.lm.services.project.strings.messages.projectAction",
                "Project action"
            },
            {
                "lp.app.lm.services.project.strings.messages.openingProject",
                "Opening project at location {0}..."
            },
            {
                "lp.app.lm.services.project.strings.messages.openedProject",
                "Opened project {0} at location {1}"
            },
            {
                "lp.app.lm.services.project.strings.messages.savingProject",
                "Saving project {0} to location {1}..."
            },
            {
                "lp.app.lm.services.project.strings.messages.savedProject",
                "Saved project {0} to location {1}"
            },
            {
                "lp.app.lm.services.project.strings.messages.buildingProject",
                "Building project {0} to location {1}..."
            },
            {
                "lp.app.lm.services.project.strings.messages.builtProject",
                "Built project {0} to location {1}"
            },
            {
                "lp.app.lm.services.project.strings.messages.closedProject",
                "Closed project {0}"
            },

            {
                "lp.app.lm.services.project.strings.messages.errorFormat",
                "Error: {0}"
            },
        };

        public string Category { get; private set; }
        public string CreatedProjectMessage { get; private set; }
        public string ProjectAction { get; private set; }
        public string OpeningProjectMessage { get; private set; }
        public string OpenedProjectMessage { get; private set; }
        public string SavingProjectMessage { get; private set; }
        public string SavedProjectMessage { get; private set; }
        public string BuildingProjectMessage { get; private set; }
        public string BuiltProjectMessage { get; private set; }
        public string ClosedProjectMessage { get; private set; }
        public string ErrorMessageFormat { get; private set; }

        public void Update(ILpDesktopAppLocalizationService service)
        {
            var localization = service.GetLocalizedStrings(KeysAndDefaults);
            Category = localization["lp.app.lm.services.project.strings.category"];
            CreatedProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.createdProject"];
            ProjectAction =
                localization["lp.app.lm.services.project.strings.messages.projectAction"];
            OpeningProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.openingProject"];
            OpenedProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.openedProject"];
            SavingProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.savingProject"];
            SavedProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.savedProject"];
            BuildingProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.buildingProject"];
            BuiltProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.builtProject"];
            ClosedProjectMessage =
                localization["lp.app.lm.services.project.strings.messages.closedProject"];
            ErrorMessageFormat =
                localization["lp.app.lm.services.project.strings.messages.errorFormat"];
        }        
    }
    #endregion

    #region Service
    public class LmProjectService : ILmProjectService
    {
        #region Internals & ctor & Dispose
        private readonly LmProjectServiceLocalization localization = new();
        private readonly IValidationService validationService;
        private readonly IFsService fsService;
        private readonly IJsonDocumentService jsonDocumentService;
        private readonly ILpDesktopAppLocalizationService localizationService;
        private readonly ILpAppStatusBarService statusBarService;

        private string currentProjectFilePath;

        public LmProjectService(
            IValidationService validationService,
            IFsService fsService,
            IJsonDocumentService jsonDocumentService,
            ILpDesktopAppLocalizationService localizationService,
            ILpAppStatusBarService statusBarService)
        {
            this.validationService = validationService;
            this.fsService = fsService;
            this.jsonDocumentService = jsonDocumentService;
            this.localizationService = localizationService;
            this.statusBarService = statusBarService;
            localizationService.AddConsumer(this);
            localization.Update(localizationService);
        }

        public void Dispose()
        {
            localizationService.DeleteConsumer(this);
        }
        #endregion

        #region ILpDesktopAppLocalizationConsumer
        public void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            localization.Update(service);
        }
        #endregion

        #region Interface impl
        public LmProjectDocument CurrentProject { get; private set; }
        public event Action CurrentProjectChanged;

        #region Create/Update/Open/Save/Build/Close Project
        public async Task CreateProjectAsync(
            CreateProjectRequest request,
            CancellationToken cancellationToken = default)
        {
            validationService.EnsureValid(request, new CreateProjectRequestValidator());

            var project = new LmProjectDocument
            {
                Name = request.Name,
                Description = request.Description,
                Source = request.Source,
                Locale = request.Locale,
            };

            var r = await statusBarService.ExceptionGuardAsync(
                async () =>
                {
                    await jsonDocumentService.WriteDocumentToFileAsync(
                        new JsonDocument<LmProjectDocument>
                        {
                            DocType = LmProjectDocument.DocType,
                            DocVersion = LmProjectDocument.DocVersion,
                            Document = project
                        },
                        request.FilePath,
                        true,
                        LmProjectDocument.JsonConverters,
                        cancellationToken);
                    CurrentProject = project;
                    currentProjectFilePath = request.FilePath;
                    SetHasUnsavedData(false);

                    statusBarService.AddMessage(new()
                    {
                        Category = localization.Category,
                        Message = string.Format(
                            localization.CreatedProjectMessage,
                            CurrentProject.Name,
                            currentProjectFilePath)
                    });
                },
                localization.Category,
                localization.ErrorMessageFormat);
            if (r == SafeAwaitResult.Success)
            {
                CurrentProjectChanged?.Invoke();
            }
        }

        public void UpdateProject(UpdateProjectRequest request)
        {
            validationService.EnsureValid(request, new UpdateProjectRequestValidator());

            CurrentProject.Name = request.Name;
            CurrentProject.Description = request.Description;
            CurrentProject.Source = request.Source;
            CurrentProject.Locale = request.Locale;
            SetHasUnsavedData(true);
        }

        public async Task OpenProjectAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var (task, _) = statusBarService.CreatePendingTask(
                localization.Category,
                localization.ProjectAction,
                string.Format(localization.OpeningProjectMessage, filePath),
                async (ctx, ct) =>
                {
                    ctx.SetProgressIndeterminate(true);
                    var doc = await jsonDocumentService.ReadDocumentFromFileAsync
                    <LmProjectDocument>(
                        filePath,
                        LmProjectDocument.DocType,
                        LmProjectDocument.DocVersion,
                        null,
                        LmProjectDocument.JsonConverters,
                        cancellationToken);
                    CurrentProject = doc.Document;
                    currentProjectFilePath = filePath;
                    SetHasUnsavedData(false);
                    statusBarService.AddMessage(new()
                    {
                        Category = localization.Category,
                        Message = string.Format(
                            localization.OpenedProjectMessage,
                            CurrentProject.Name,
                            currentProjectFilePath)
                    });
                });
            var r = await task;
            if (r == SafeAwaitResult.Success)
            {
                CurrentProjectChanged?.Invoke();
            }
        }

        public Task SaveProjectAsync(CancellationToken cancellationToken = default) =>
            SaveProjectAsAsync(currentProjectFilePath, cancellationToken);

        public async Task SaveProjectAsAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var (task, _) = statusBarService.CreatePendingTask(
                localization.Category,
                localization.ProjectAction,
                string.Format(localization.SavingProjectMessage, CurrentProject.Name, filePath),
                async (ctx, ct) =>
                {
                    ctx.SetProgressIndeterminate(true);
                    await jsonDocumentService.WriteDocumentToFileAsync(
                        new JsonDocument<LmProjectDocument>
                        {
                            DocType = LmProjectDocument.DocType,
                            DocVersion = LmProjectDocument.DocVersion,
                            Document = CurrentProject
                        },
                        filePath,
                        true,
                        LmProjectDocument.JsonConverters,
                        cancellationToken);
                    currentProjectFilePath = filePath;
                    SetHasUnsavedData(false);
                    statusBarService.AddMessage(new()
                    {
                        Category = localization.Category,
                        Message = string.Format(
                            localization.SavedProjectMessage,
                            CurrentProject.Name,
                            currentProjectFilePath)
                    });
                });
            await task;
        }

        public async Task BuildProjectAsync(CancellationToken cancellationToken = default)
        {
            var targetFilePath =
                fsService.PathCombine(
                    fsService.PathGetDirectoryName(currentProjectFilePath),
                    LpLocalizationDocument.GetFileName(
                        CurrentProject.Source,
                        CurrentProject.Locale));
            var doc = new JsonDocument<LpLocalizationDocument>()
            {
                DocType = LpLocalizationDocument.DocType,
                DocVersion = LpLocalizationDocument.DocVersion,
                Document = new()
                {
                    Items = CurrentProject.Items
                        .Select(i =>
                        {
                            AbstractLpLocalizationResourceDocumentEntry r = i.ResourceType switch
                            {
                                LpLocalizationResourceType.String =>
                                    new LpStringLocalizationResourceDocumentEntry(i.Key)
                                    {
                                        Value = i.Transaltion
                                    },
                                LpLocalizationResourceType.File =>
                                    new LpFileLocalizationResourceDocumentEntry(i.Key)
                                    {
                                        Path = i.Transaltion
                                    },
                                _ => throw new NotSupportedException()
                            };
                            return r;
                        })
                        .ToList()
                }
            };
            var (task, _) = statusBarService.CreatePendingTask(
                localization.Category,
                localization.ProjectAction,
                string.Format(
                    localization.BuildingProjectMessage,
                    CurrentProject.Name,
                    targetFilePath),
                async (ctx, ct) =>
                {
                    ctx.SetProgressIndeterminate(true);
                    await jsonDocumentService.WriteDocumentToFileAsync(doc,
                        targetFilePath,
                        true,
                        LpLocalizationDocument.JsonConverters,
                        cancellationToken);
                    statusBarService.AddMessage(new()
                    {
                        Category = localization.Category,
                        Message = string.Format(
                            localization.BuiltProjectMessage,
                            CurrentProject.Name,
                            targetFilePath)
                    });
                });
            await task;
        }

        public void CloseProject()
        {
            statusBarService.AddMessage(new()
            {
                Category = localization.Category,
                Message = string.Format(
                    localization.ClosedProjectMessage,
                    CurrentProject.Name)
            });
            CurrentProject = null;
            currentProjectFilePath = null;
            SetHasUnsavedData(false);
            CurrentProjectChanged?.Invoke();
        }
        #endregion

        #region Items management
        public LmProjectDocument.Item CreateItem(CreateItemRequest request)
        {
            validationService.EnsureValid(
                request,
                new CreateItemRequestValidator(
                    CurrentProject.Items.Select(i => i.Key).ToHashSet()));
            var item = new LmProjectDocument.Item
            {
                ResourceType = request.ResourceType,
                Key = request.Key
            };
            CurrentProject.Items.Add(item);
            SetHasUnsavedData(true);
            return item;
        }

        public PaginatedList<LmProjectDocument.Item> GetItems(
            ILmProjectService.ItemsFilter filter,
            ILmProjectService.ItemsOrder? order = ILmProjectService.ItemsOrder.KeyAsc,
            int page = 1,
            int pageSize = 50)
        {
            var e = CurrentProject.Items.AsEnumerable();
            if (filter.Status.HasValue)
            {
                e = e.Where(i => i.Status == filter.Status.Value);
            }

            if (filter.ResourceType.HasValue)
            {
                e = e.Where(i => i.ResourceType == filter.ResourceType.Value);
            }

            if (!string.IsNullOrEmpty(filter.Key))
            {
                e = e.Where(i => i.Key.Contains(filter.Key, StringComparison.Ordinal));
            }

            e = order switch
            {
                ILmProjectService.ItemsOrder.StatusAsc => e.OrderBy(i => i.Status),
                ILmProjectService.ItemsOrder.StatusDesc => e.OrderByDescending(i => i.Status),
                ILmProjectService.ItemsOrder.ResourceTypeAsc => e.OrderBy(i => i.ResourceType),
                ILmProjectService.ItemsOrder.ResourceTypeDesc =>
                    e.OrderByDescending(i => i.ResourceType),
                ILmProjectService.ItemsOrder.KeyAsc => e.OrderBy(i => i.Key),
                ILmProjectService.ItemsOrder.KeyDesc => e.OrderByDescending(i => i.Key),
                null => e,
                _ => throw new NotSupportedException()
            };
            return e.ToPaginatedList(pageSize, page);
        }

        public LmProjectDocument.Item GetItem(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return null;
            }

            return CurrentProject
                .Items
                .FirstOrDefault(i => i.Key.Equals(key, StringComparison.Ordinal));
        }

        public void UpdateItem(
            string key,
            LmProjectDocument.ItemStatus status,
            string translation)
        {
            var item = GetItem(key);
            item.Status = status;
            item.Transaltion = translation;
            SetHasUnsavedData(true);
        }

        public void UpdateItems(
            ILmProjectService.ItemsFilter filter,
            ILmProjectService.ItemsOrder? order,
            int page,
            int pageSize,
            LmProjectDocument.ItemStatus status)
        {
            GetItems(filter, order, page, pageSize)
                .Items
                .ForEach(i => i.Status = status);
            SetHasUnsavedData(true);
        }

        public void UpdateAllItems(LmProjectDocument.ItemStatus status)
        {
            CurrentProject
                .Items
                .ForEach(i => i.Status = status);
            SetHasUnsavedData(true);
        }

        public void DeleteItem(string key)
        {
            var item = GetItem(key);
            if (item == null)
            {
                return;
            }
            CurrentProject.Items.Remove(item);
            SetHasUnsavedData(true);
        }

        public void DeleteItems(
            ILmProjectService.ItemsFilter filter,
            ILmProjectService.ItemsOrder? order,
            int page,
            int pageSize)
        {
            GetItems(filter, order, page, pageSize)
                .Items
                .ForEach(i => CurrentProject.Items.Remove(i));
            SetHasUnsavedData(true);
        }

        public void DeleteAllItems()
        {
            CurrentProject.Items.Clear();
            SetHasUnsavedData(true);
        }

        public async Task<bool> UseBasisAsync(
            string filePath,
            CancellationToken cancellationToken = default)
        {
            var basisDoc = await localizationService.LoadDocumentAsync(filePath, cancellationToken);
            if (basisDoc == null)
            {
                return false;
            }

            foreach (var basisItem in basisDoc.Items)
            {
                var item = CurrentProject
                    .Items
                    .FirstOrDefault(e => e.Key.Equals(basisItem.Key, StringComparison.Ordinal));
                if (item == null)
                {
                    item = new LmProjectDocument.Item
                    {
                        ResourceType = basisItem.Type,
                        Key = basisItem.Key,
                        Transaltion = GetBasisItemTranslation(basisItem)
                    };
                    CurrentProject.Items.Add(item);
                }
                else
                {
                    item.Status = LmProjectDocument.ItemStatus.Review;
                }

                item.Basis = GetBasisItemTranslation(basisItem);
            }

            SetHasUnsavedData(true);
            return true;
        }
        #endregion
        #endregion

        #region Internals
        private static string GetBasisItemTranslation(
            AbstractLpLocalizationResourceDocumentEntry basisItem)
        {
            return basisItem.Type switch
            {
                LpLocalizationResourceType.String =>
                    ((LpStringLocalizationResourceDocumentEntry)basisItem).Value,
                LpLocalizationResourceType.File =>
                    ((LpFileLocalizationResourceDocumentEntry)basisItem).Path,
                _ => throw new NotSupportedException()
            };
        }

        private void SetHasUnsavedData(bool value)
        {
            Application.Current.GetLpApplicationRoot().HasUnsavedData = value;
        }
        #endregion
    }
    #endregion
}
