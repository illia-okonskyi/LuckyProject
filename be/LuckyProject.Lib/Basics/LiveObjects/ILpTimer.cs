using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects
{
    public interface ILpTimer : IDisposable
    {
        #region Properties
        string Name { get; }
        TimeSpan Interval { get; }
        bool SingleShot { get; }
        bool StopOnShot { get; }
        bool StopOnException { get; }
        bool IsStarted { get; }
        #endregion

        #region Methods
        void Start();
        void Stop();
        /// <summary>
        /// Returns local exit code
        /// </summary>
        Task<int> ForceTickAsync(CancellationToken cancellationToken = default);
        /// <summary>
        /// Returns local exit code
        /// </summary>
        int ForceTick(CancellationToken cancellationToken = default);
        #endregion
    }
}
