using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Helpers
{
    public static class TaskHelper
    {
        public static async Task<bool> SafeDelay(TimeSpan ts, CancellationToken ct = default)
        {
            if (ct.IsCancellationRequested)
            {
                return false;
            } 

            try
            {
                await Task.Delay(ts, ct);
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
