using LuckyProject.Lib.Basics.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    /// <summary>
    /// Default timeouts are same as for Helper: <see cref="ThreadSyncHelper"/>
    /// </summary>
    /// <remarks>
    /// NOTE: Do not add Interlocked functionality forwarding: it assumes atomic operation and
    ///       service (+virtual call) forwarwarding will break operations atomarity
    /// </remarks>
    public interface IThreadSyncService
    {
        #region Monitor
        bool MonitorGuard(object o, Action a, TimeSpan? timeout = null);
        (bool Enterted, T Result) MonitorGuard<T>(
            object o,
            Func<T> f,
            TimeSpan? timeout = null);
        Task<bool> MonitorSpinWaitAsync(
            object o,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default);
        #endregion

        #region SpinLock
        bool SpinLockGuard(SpinLock sl, Action a, TimeSpan? timeout = null);
        (bool Enterted, T Result) SpinLockGuard<T>(
            SpinLock sl,
            Func<T> f,
            TimeSpan? timeout = null);
        Task<bool> SpinLockSpinWaitAsync(
            SpinLock sl,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default);
        #endregion

        #region WaitHandle
        bool WaitHandleSignalAndWait(
            WaitHandle toSignal,
            WaitHandle toWaitOn,
            TimeSpan? timeout = null,
            bool exitContext = false);
        bool WaitHandleWaitAll(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false);
        int WaitHandleWaitAny(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false);
        bool WaitHandleWaitOne(
            WaitHandle waitHandle,
            TimeSpan? timeout = null,
            bool exitContext = false);
        #endregion

        #region Mutex
        bool MutexGuard(Mutex m, Action a, TimeSpan? timeout = null);
        (bool Enterted, T Result) MutexGuard<T>(
            Mutex m,
            Func<T> f,
            TimeSpan? timeout = null);
        Task<bool> MutexSpinWaitAsync(
            Mutex m,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default);
        #endregion

        #region SemaphoreSlim
        [UnsupportedOSPlatform("browser")]
        bool SemaphoreSlimGuard(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default);

        [UnsupportedOSPlatform("browser")]
        (bool Enterted, T Result) SemaphoreSlimGuard<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        Task<bool> SemaphoreSlimGuardAsync(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<T>> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default);
        Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<bool>> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default);
        #endregion

        #region Thread
        bool ThreadJoin(Thread t, TimeSpan? timeout = null);
        #endregion

        #region ManualResetEventSlim
        [UnsupportedOSPlatform("browser")]
        bool ManualResetEventSlimWait(
            ManualResetEventSlim mres,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        #endregion

        #region CountdownEvent
        [UnsupportedOSPlatform("browser")]
        bool CountdownEventWait(
            CountdownEvent ce,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        #endregion

        #region Barrier
        [UnsupportedOSPlatform("browser")]
        bool BarrierSignalAndWait(
            Barrier b,
            TimeSpan? timeout = null,
            CancellationToken ct = default);
        #endregion

        #region ReaderWriterLockSlim
        bool ReaderWriterLockSlimReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null);

        (bool Entered, T Result) ReaderWriterLockSlimReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null);

        Task<bool> ReaderWriterLockSlimReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default);

        bool ReaderWriterLockSlimUpgradeableReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null);

        (bool Entered, T Result) ReaderWriterLockSlimUpgradeableReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null);

        Task<bool> ReaderWriterLockSlimUpgradeableReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default);

        bool ReaderWriterLockSlimWriteGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null);

        (bool Entered, T Result) ReaderWriterLockSlimWriteGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null);
        #endregion
    }
}
