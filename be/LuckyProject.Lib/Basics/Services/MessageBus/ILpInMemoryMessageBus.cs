using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services.MessageBus
{
    public interface ILpInMemoryMessageBus
    {
        ILpInMemoryMessageBusSubscriber CreateSubscriber(
            Func<object, CancellationToken, Task> handleAsync,
            Func<object, bool> canHandle = null,
            string name = null);
        ILpInMemoryMessageBusSubscriber CreateSubscriber<TMessage>(
            Func<TMessage, CancellationToken, Task> handleAsync,
            Func<TMessage, bool> canHandle = null,
            string name = null);

        void Subscribe(ILpInMemoryMessageBusSubscriber subscriber);
        bool Unsubscribe(ILpInMemoryMessageBusSubscriber subscriber);
        void UnsubscribeAll();
        List<Task> PostMessage(
            object message,
            CancellationToken messageCancellationToken = default);
        Task SendMessageAsync(object message, CancellationToken cancellationToken = default);
    }
}
