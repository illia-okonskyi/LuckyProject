using LuckyProject.Lib.Basics.Collections;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.Services.Project.Requests;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.Services.Project
{
    public interface ILmProjectService : ILpDesktopAppLocalizationConsumer
    {
        LmProjectDocument CurrentProject { get; }
        event Action CurrentProjectChanged;

        #region Create/Update/Open/Save/Build/Close Project
        Task CreateProjectAsync(
            CreateProjectRequest request,
            CancellationToken cancellationToken = default);
        void UpdateProject(UpdateProjectRequest request);
        Task OpenProjectAsync(
            string filePath,
            CancellationToken cancellationToken = default);
        Task SaveProjectAsync(CancellationToken cancellationToken = default);
        Task SaveProjectAsAsync(
            string filePath,
            CancellationToken cancellationToken = default);
        Task BuildProjectAsync(CancellationToken cancellationToken = default);
        void CloseProject();
        #endregion

        #region Items management
        public class ItemsFilter
        {
            public LmProjectDocument.ItemStatus? Status { get; set; }
            public LpLocalizationResourceType? ResourceType { get; set; }
            public string Key { get; set; }
        }

        public enum ItemsOrder
        {
            StatusAsc,
            StatusDesc,
            ResourceTypeAsc,
            ResourceTypeDesc,
            KeyAsc,
            KeyDesc,
        }

        LmProjectDocument.Item CreateItem(CreateItemRequest request);
        PaginatedList<LmProjectDocument.Item> GetItems(
            ItemsFilter filter,
            ItemsOrder? order = ItemsOrder.KeyAsc,
            int page = 1,
            int pageSize = 50);
        LmProjectDocument.Item GetItem(string key);
        void UpdateItem(
            string key,
            LmProjectDocument.ItemStatus status,
            string translation);
        void UpdateItems(
            ItemsFilter filter,
            ItemsOrder? order,
            int page,
            int pageSize,
            LmProjectDocument.ItemStatus status);
        void UpdateAllItems(LmProjectDocument.ItemStatus status);
        void DeleteItem(string key);
        void DeleteItems(
            ItemsFilter filter,
            ItemsOrder? order,
            int page,
            int pageSize);
        void DeleteAllItems();
        Task<bool> UseBasisAsync(
            string filePath,
            CancellationToken cancellationToken = default);
        #endregion
    }
}
