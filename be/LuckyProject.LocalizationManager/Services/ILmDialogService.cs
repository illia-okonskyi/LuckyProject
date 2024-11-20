using LuckyProject.LocalizationManager.Models;
using LuckyProject.LocalizationManager.ViewModels.Dialogs;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.LocalizationManager.Services
{
    public interface ILmDialogService
    {
        Task<bool> CreateUpdateProjectAsync(CreateUpdateProjectMode mode);
        Task<bool> UseProjectBasisAsync(CancellationToken cancellationToken = default);
        Task<bool> OpenProjectAsync(CancellationToken cancellationToken = default);
        Task<bool> SaveProjectAsAsync(CancellationToken cancellationToken = default);
        Task<bool> CloseProjectAsync(string confirmTitle, string confirmText);
        Task<LmProjectDocument.Item> CreateProjectItemAsync();
    }
}
