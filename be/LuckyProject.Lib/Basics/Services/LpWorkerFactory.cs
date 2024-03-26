using LuckyProject.Lib.Basics.LiveObjects.Workers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpWorkerFactory : ILpWorkerFactory
    {
        public ILpWorker CreateCallbackWorker(
            Func<LpWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null)
        {
            return new LpCallbackWorker(callback, name, logger);
        }

        public ILpPausableWorker CreatePausableCallbackWorker(
            Func<LpPausableWorkerCallbackContext, Task> callback,
            string name = null,
            ILogger logger = null)
        {
            return new LpPausableCallbackWorker(callback, name, logger);
        }

        public ILpPausableWorker CreatePausableStateCallbackWorker<TState>(
            Func<LpPausableStateWorkerCallbackContext<TState>, Task> callback,
            TState initialState = default,
            string name = null,
            ILogger logger = null)
        {
            return new LpPauseableStateCallbackWorker<TState>(callback, initialState, name, logger);
        }
    }
}
