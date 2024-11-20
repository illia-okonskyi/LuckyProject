using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IAppSettingsService<TSettings> : IDisposable
        where TSettings : class, new()
    {
        TSettings CurrentSettings { get; }
        Task LoadAsync(CancellationToken cancellationToken = default);
        Task SaveAsync(CancellationToken cancellationToken = default);
    }
}
