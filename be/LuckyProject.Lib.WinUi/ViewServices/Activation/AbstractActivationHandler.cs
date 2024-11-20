using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Activation
{
    public abstract class AbstractActivationHandler<T> : IActivationHandler
        where T : class
    {
        /// <summary>
        /// Override this method to add the logic for whether to handle the activation.
        /// </summary>
        protected virtual bool CanHandleInternal(T args) => true;

        /// <summary>
        /// Override this method to add the logic for your activation handler.
        /// </summary>
        protected abstract Task HandleInternalAsync(T args);

        public bool CanHandle(object args) => args is T && CanHandleInternal(args as T);

        public async Task HandleAsync(object args) => await HandleInternalAsync(args as T);
    }
}

