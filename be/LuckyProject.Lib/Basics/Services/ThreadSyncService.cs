using LuckyProject.Lib.Basics.Helpers;
using System;
using System.Collections.Generic;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public class ThreadSyncService : IThreadSyncService
    {
        #region Monitor
        public bool MonitorGuard(object o, Action a, TimeSpan? timeout = null) =>
            ThreadSyncHelper.MonitorGuard(o, a, timeout);
        public (bool Enterted, T Result) MonitorGuard<T>(
            object o,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.MonitorGuard(o, f, timeout);
        public Task<bool> MonitorSpinWaitAsync(
            object o,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.MonitorSpinWaitAsync(o, pred, guardTimeout, spinWaitTimeout, ct);
        #endregion

        #region SpinLock
        public bool SpinLockGuard(SpinLock sl, Action a, TimeSpan? timeout = null) =>
            ThreadSyncHelper.SpinLockGuard(sl, a, timeout);
        public (bool Enterted, T Result) SpinLockGuard<T>(
            SpinLock sl,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.SpinLockGuard(sl, f, timeout);
        public Task<bool> SpinLockSpinWaitAsync(
            SpinLock sl,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SpinLockSpinWaitAsync(sl, pred, guardTimeout, spinWaitTimeout, ct);
        #endregion

        #region WaitHandle
        public bool WaitHandleSignalAndWait(
            WaitHandle toSignal,
            WaitHandle toWaitOn,
            TimeSpan? timeout = null,
            bool exitContext = false) =>
            ThreadSyncHelper.WaitHandleSignalAndWait(toSignal, toWaitOn, timeout, exitContext);
        public bool WaitHandleWaitAll(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false) =>
            ThreadSyncHelper.WaitHandleWaitAll(waitHandles, timeout, exitContext);
        public int WaitHandleWaitAny(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false) =>
            ThreadSyncHelper.WaitHandleWaitAny(waitHandles, timeout, exitContext);
        public bool WaitHandleWaitOne(
            WaitHandle waitHandle,
            TimeSpan? timeout = null,
            bool exitContext = false) =>
            ThreadSyncHelper.WaitHandleWaitOne(waitHandle, timeout, exitContext);
        #endregion

        #region Mutex
        public bool MutexGuard(Mutex m, Action a, TimeSpan? timeout = null) =>
            ThreadSyncHelper.MutexGuard(m, a, timeout);
        public (bool Enterted, T Result) MutexGuard<T>(
            Mutex m,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.MutexGuard(m, f, timeout);
        public Task<bool> MutexSpinWaitAsync(
            Mutex m,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.MutexSpinWaitAsync(m, pred, guardTimeout, spinWaitTimeout, ct);
        #endregion

        #region SemaphoreSlim
        [UnsupportedOSPlatform("browser")]
        public bool SemaphoreSlimGuard(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimGuard(ss, a, timeout, ct);

        [UnsupportedOSPlatform("browser")]
        public (bool Enterted, T Result) SemaphoreSlimGuard<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimGuard(ss, f, timeout, ct);
        public Task<bool> SemaphoreSlimGuardAsync(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimGuardAsync(ss, a, timeout, ct);
        public Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimGuardAsync(ss, f, timeout, ct);
        public Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<T>> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimGuardAsync(ss, f, timeout, ct);
        public Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimSpinWaitAsync(
                ss,
                pred,
                guardTimeout,
                spinWaitTimeout,
                ct);
        public Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<bool>> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.SemaphoreSlimSpinWaitAsync(
                ss,
                pred,
                guardTimeout,
                spinWaitTimeout,
                ct);
        #endregion

        #region Thread
        public bool ThreadJoin(Thread t, TimeSpan? timeout = null) =>
            ThreadSyncHelper.ThreadJoin(t, timeout);
        #endregion

        #region ManualResetEventSlim
        [UnsupportedOSPlatform("browser")]
        public bool ManualResetEventSlimWait(
            ManualResetEventSlim mres,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.ManualResetEventSlimWait(mres, timeout, ct);
        #endregion

        #region CountdownEvent
        [UnsupportedOSPlatform("browser")]
        public bool CountdownEventWait(
            CountdownEvent ce,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.CountdownEventWait(ce, timeout, ct);
        #endregion

        #region Barrier
        [UnsupportedOSPlatform("browser")]
        public bool BarrierSignalAndWait(
            Barrier b,
            TimeSpan? timeout = null,
            CancellationToken ct = default) =>
            ThreadSyncHelper.BarrierSignalAndWait(b, timeout, ct);
        #endregion

        #region ReaderWriterLockSlim
        public bool ReaderWriterLockSlimReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimReadGuard(rwls, a, timeout);

        public (bool Entered, T Result) ReaderWriterLockSlimReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimReadGuard(rwls, f, timeout);

        public Task<bool> ReaderWriterLockSlimReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default) =>
            ThreadSyncHelper.ReaderWriterLockSlimReadSpinWaitAsync(
                rwls,
                pred,
                guardTimeout,
                spinWaitTimeout,
                ct);

        public bool ReaderWriterLockSlimUpgradeableReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimUpgradeableReadGuard(rwls, a, timeout);

        public (bool Entered, T Result) ReaderWriterLockSlimUpgradeableReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimUpgradeableReadGuard(rwls, f, timeout);

        public Task<bool> ReaderWriterLockSlimUpgradeableReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default) =>
            ThreadSyncHelper.ReaderWriterLockSlimUpgradeableReadSpinWaitAsync(
                rwls,
                pred,
                guardTimeout,
                spinWaitTimeout,
                ct);

        public bool ReaderWriterLockSlimWriteGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimWriteGuard(rwls, a, timeout);

        public (bool Entered, T Result) ReaderWriterLockSlimWriteGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null) =>
            ThreadSyncHelper.ReaderWriterLockSlimWriteGuard(rwls, f, timeout);
        #endregion
    }
}
