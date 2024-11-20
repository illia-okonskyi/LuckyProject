using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services.MessageBus
{
    public class LpInMemoryMessageBusSubscriber : ILpInMemoryMessageBusSubscriber
    {
        private const string DefaultName = "Noname";

        private readonly Func<object, bool> canHandle;
        private readonly Func<object, CancellationToken, Task> handleAsync;
        private readonly ILogger logger;
        public string Name { get; }

        public LpInMemoryMessageBusSubscriber(
            Func<object, bool> canHandle,
            Func<object, CancellationToken, Task> handleAsync,
            ILogger logger,
            string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                name = DefaultName;
            }

            this.canHandle = canHandle;
            this.handleAsync = handleAsync;
            this.logger = logger;
            Name = name;
        }

        public virtual void Dispose()
        { }

        public bool CanHandle(object message)
        {
            if (canHandle == null)
            {
                return true;
            }

            try
            {
                return canHandle(message);
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    $"Error in InMemory Message Bus Subscriber {Name}.CanHandle; Message Rejected");
                return false;
            }
        }

        public async Task HandleAsync(
            object message,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await handleAsync(message, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                // NOTE: do nothing, just swallow
            }
            catch (Exception ex)
            {
                logger.LogWarning(
                    ex,
                    $"Error in InMemory Message Bus Subscriber {Name}.HandleAsync");
            }
        }
    }

    public class LpInMemoryMessageBusSubscriber<TMessage> : LpInMemoryMessageBusSubscriber
    {
        public LpInMemoryMessageBusSubscriber(
            Func<TMessage, bool> canHandle,
            Func<TMessage, CancellationToken, Task> handleAsync,
            ILogger logger,
            string name)
            : base(
                  CreateCanHandle(canHandle),
                  CreateHandleAsync(handleAsync),
                  logger,
                  name)
        { }

        private static Func<object, bool> CreateCanHandle(Func<TMessage, bool> canHandle)
        {
            if (canHandle == null)
            {
                return null;
            }

            return m => m is TMessage && canHandle((TMessage)m);
        }

        private static Func<object, CancellationToken, Task> CreateHandleAsync(
            Func<TMessage, CancellationToken, Task> handleAsync)
        {
            return (m, ct) =>
            {
                if (m is TMessage message)
                {
                    return handleAsync((TMessage)m, ct);
                }

                return Task.CompletedTask;
            };
        }
    }
}
