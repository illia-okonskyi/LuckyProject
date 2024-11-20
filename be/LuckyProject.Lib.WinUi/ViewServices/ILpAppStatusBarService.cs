using LuckyProject.Lib.WinUi.Models;
using System.Threading.Tasks;
using System.Threading;
using System;
using LuckyProject.Lib.Basics.Extensions;

namespace LuckyProject.Lib.WinUi.Services
{
    public interface ILpAppStatusBarService
    {
        bool IsBusy { get; }
        public (Task<SafeAwaitResult> Task, Func<Task> CancelTaskAsync) CreatePendingTask(
            string category,
            string description,
            string status,
            Func<LpAppTaskContext, CancellationToken, Task> handler,
            bool isCancelable = false,
            LpAppMessageType errorMessageType = LpAppMessageType.Error);
        void AddMessage(LpAppMessage message);
        bool ExceptionGuard(
            Action action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error);
        (bool Success, T Value) ExceptionGuard<T>(
            Func<T> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error);
        Task<SafeAwaitResult> ExceptionGuardAsync(
            Func<Task> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error);
        Task<(SafeAwaitResult Result, T Value)> ExceptionGuardAsync<T>(
            Func<Task<T>> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error);
    }
}
