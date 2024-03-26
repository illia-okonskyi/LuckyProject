using LuckyProject.Lib.Basics.LiveObjects.Workers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpWorkerFactory
    {
        ILpWorker CreateCallbackWorker(
            Func<LpWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null);
        ILpPausableWorker CreatePausableCallbackWorker(
            Func<LpPausableWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null);
        ILpPausableWorker CreatePausableStateCallbackWorker<TState>(
            Func<LpPausableStateWorkerCallbackContext<TState>, Task> callback,
            TState initialState = default,
            string name = null,
            ILogger logger = null);
    }
}
