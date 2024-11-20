using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Activation
{
    public interface IActivationHandler
    {
        bool CanHandle(object args);
        Task HandleAsync(object args);
    }
}