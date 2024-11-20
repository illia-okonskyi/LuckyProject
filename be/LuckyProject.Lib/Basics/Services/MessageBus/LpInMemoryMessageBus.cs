using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services.MessageBus
{
    public class LpInMemoryMessageBus : ILpInMemoryMessageBus
    {
        #region Internals
        private readonly IThreadSyncService tsService;
        private readonly ILogger logger;

        private readonly List<ILpInMemoryMessageBusSubscriber> subscribers = new();
        private readonly ReaderWriterLockSlim subscribersLock = new();
        #endregion

        #region ctor
        public LpInMemoryMessageBus(
            IThreadSyncService tsService,
            ILogger<LpInMemoryMessageBus> logger)
        {
            this.tsService = tsService;
            this.logger = logger;
        }
        #endregion

        #region Public interface
        public ILpInMemoryMessageBusSubscriber CreateSubscriber(
            Func<object, CancellationToken, Task> handleAsync,
            Func<object, bool> canHandle = null,
            string name = null)
        {
            return new LpInMemoryMessageBusSubscriber(canHandle, handleAsync, logger, name);
        }

        public ILpInMemoryMessageBusSubscriber CreateSubscriber<TMessage>(
            Func<TMessage, CancellationToken, Task> handleAsync,
            Func<TMessage, bool> canHandle = null,
            string name = null)
        {
            return new LpInMemoryMessageBusSubscriber<TMessage>(
                canHandle,
                handleAsync,
                logger,
                name);
        }

        public void Subscribe(ILpInMemoryMessageBusSubscriber subscriber)
        {
            tsService.ReaderWriterLockSlimWriteGuard(
                subscribersLock,
                () => subscribers.Add(subscriber));
        }

        public bool Unsubscribe(ILpInMemoryMessageBusSubscriber subscriber)
        {
            var (_, removed) = tsService.ReaderWriterLockSlimWriteGuard(
                subscribersLock,
                () => subscribers.Remove(subscriber));
            return removed;
        }

        public void UnsubscribeAll()
        {
            tsService.ReaderWriterLockSlimWriteGuard(
             subscribersLock,
             () => subscribers.Clear());
        }

        public List<Task> PostMessage(
            object message,
            CancellationToken messageCancellationToken = default)
        {
            var (_, subscribers) = tsService.ReaderWriterLockSlimReadGuard(
                subscribersLock,
                () => this.subscribers.ToList());

            subscribers = subscribers.Where(s => s.CanHandle(message)).ToList();
            return subscribers
                .Select(s => Task.Run(
                    () => s.HandleAsync(message, messageCancellationToken),
                    messageCancellationToken))
                .ToList();
        }

        public Task SendMessageAsync(
            object message,
            CancellationToken cancellationToken = default) =>
            Task.WhenAll(PostMessage(message, cancellationToken));
        #endregion
    }
}
