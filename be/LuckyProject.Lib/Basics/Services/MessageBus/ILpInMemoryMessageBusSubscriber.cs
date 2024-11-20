using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services.MessageBus
{
    public interface ILpInMemoryMessageBusSubscriber : IDisposable
    {
        string Name { get; }
        bool CanHandle(object message);
        Task HandleAsync(object message, CancellationToken cancellationToken = default);
    }
}
