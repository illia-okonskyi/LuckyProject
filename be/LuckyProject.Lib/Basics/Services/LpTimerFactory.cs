using LuckyProject.Lib.Basics.LiveObjects;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class LpTimerFactory : ILpTimerFactory
    {
        public ILpTimer CreateTimer(
            TimeSpan interval,
            bool singleShot,
            Action<DateTime, CancellationToken> callback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null)
        {
            return new LpTimer(
                interval,
                singleShot,
                callback,
                stopOnShot,
                stopOnException,
                name,
                logger);
        }

        public ILpTimer CreateTimer(
            TimeSpan interval,
            bool singleShot,
            Func<DateTime, CancellationToken, Task> asyncCallback,
            bool stopOnShot = true,
            bool stopOnException = true,
            string name = null,
            ILogger logger = null)
        {
            return new LpTimer(
                interval,
                singleShot,
                asyncCallback,
                stopOnShot,
                stopOnException,
                name,
                logger);

        }
    }
}
