using LuckyProject.Lib.Basics.LiveObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface ILpTimerFactory
    {
        ILpTimer CreateTimer(
            TimeSpan interval,
            bool singleShot,
            Action<DateTime, CancellationToken> callback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null);

        ILpTimer CreateTimer(
            TimeSpan interval,
            bool singleShot,
            Func<DateTime, CancellationToken, Task> asyncCallback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null);
    }
}
