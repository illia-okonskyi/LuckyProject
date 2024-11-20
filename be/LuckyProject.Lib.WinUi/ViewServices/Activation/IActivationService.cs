using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Activation
{
    public interface IActivationService
    {
        Task ActivateAsync(
            object activationArgs,
            Action shellPageSetter,
            Action mainWindowActivator);
    }
}
