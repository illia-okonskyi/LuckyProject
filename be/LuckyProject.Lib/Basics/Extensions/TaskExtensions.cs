using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Extensions
{
    public enum SafeAwaitResult
    {
        Success,
        Exception,
        Cancelled
    }

    public static class TaskExtensions
    {
        public static async Task<SafeAwaitResult> SafeAwaitWrapper(this Task task)
        {
            try
            {
                await task;
                return SafeAwaitResult.Success;
            }
            catch (OperationCanceledException)
            {
                return SafeAwaitResult.Cancelled;
            }
            catch (Exception)
            {
                return SafeAwaitResult.Exception;
            }
        }

        public static async Task<(SafeAwaitResult, T)> SafeAwaitWrapper<T>(this Task<T> task)
        {
            try
            {
                var result = await task;
                return (SafeAwaitResult.Success, result);
            }
            catch (OperationCanceledException)
            {
                return (SafeAwaitResult.Cancelled, default);
            }
            catch (Exception)
            {
                return (SafeAwaitResult.Exception, default);
            }
        }
    }
}
