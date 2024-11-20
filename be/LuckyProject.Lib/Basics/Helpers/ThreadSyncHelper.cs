using LuckyProject.Lib.Basics.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Helpers
{
    /// <summary>
    /// Default timeouts are <see cref="TimeoutDefaults.Infinity"/>, spin wait timeout is
    /// <see cref="TimeoutDefaults.Shortest"/>
    /// </summary>
    public static class ThreadSyncHelper
    {
        #region Internals
        private static Task<bool> SpinWaiter(TimeSpan? ts, CancellationToken ct) =>
            TaskHelper.SafeDelay(ts ?? TimeoutDefaults.Shortest, ct);
        #endregion

        #region Monitor
        private static bool MonitorTryEnter(object o, TimeSpan? timeout)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return Monitor.TryEnter(o, timeout.Value);
        }

        public static bool MonitorGuard(object o, Action a, TimeSpan? timeout = null)
        {
            if (!MonitorTryEnter(o, timeout))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                Monitor.Exit(o);
            }
        }

        public static (bool Enterted, T Result) MonitorGuard<T>(
            object o,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            if (!MonitorTryEnter(o, timeout))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                Monitor.Exit(o);
            }
        }

        public static async Task<bool> MonitorSpinWaitAsync(
            object o,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = MonitorGuard(o, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }
        #endregion

        #region SpinLock
        private static bool SpinLockTryEnter(SpinLock sl, TimeSpan? timeout)
        {
            timeout ??= TimeoutDefaults.Infinity;
            var gotLock = false;
            sl.TryEnter(timeout.Value, ref gotLock);
            return gotLock;
        }

        public static bool SpinLockGuard(SpinLock sl, Action a, TimeSpan? timeout = null)
        {
            if (!SpinLockTryEnter(sl, timeout))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                sl.Exit();
            }
        }

        public static (bool Enterted, T Result) SpinLockGuard<T>(
            SpinLock sl,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            if (!SpinLockTryEnter(sl, timeout))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                sl.Exit();
            }
        }

        public static async Task<bool> SpinLockSpinWaitAsync(
            SpinLock sl,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = SpinLockGuard(sl, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }
        #endregion

        #region WaitHandle
        public static bool WaitHandleSignalAndWait(
            WaitHandle toSignal,
            WaitHandle toWaitOn,
            TimeSpan? timeout = null,
            bool exitContext = false)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return WaitHandle.SignalAndWait(toSignal, toWaitOn, timeout.Value, exitContext);
        }

        public static bool WaitHandleWaitAll(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return WaitHandle.WaitAll(waitHandles.ToArray(), timeout.Value, exitContext);
        }

        public static int WaitHandleWaitAny(
            IEnumerable<WaitHandle> waitHandles,
            TimeSpan? timeout = null,
            bool exitContext = false)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return WaitHandle.WaitAny(waitHandles.ToArray(), timeout.Value, exitContext);
        }

        public static bool WaitHandleWaitOne(
            WaitHandle waitHandle,
            TimeSpan? timeout = null,
            bool exitContext = false)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return waitHandle.WaitOne(timeout.Value, exitContext);
        }
        #endregion

        #region Mutex
        public static bool MutexGuard(Mutex m, Action a, TimeSpan? timeout = null)
        {
            if (!WaitHandleWaitOne(m, timeout))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                m.ReleaseMutex();
            }
        }

        public static (bool Enterted, T Result) MutexGuard<T>(
            Mutex m,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            if (!WaitHandleWaitOne(m, timeout))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                m.ReleaseMutex();
            }
        }

        public static async Task<bool> MutexSpinWaitAsync(
            Mutex m,
            Func<bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = MutexGuard(m, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }
        #endregion

        #region SemaphoreSlim
        [UnsupportedOSPlatform("browser")]
        private static bool SemaphoreSlimWait(
            SemaphoreSlim ss,
            TimeSpan? timeout,
            CancellationToken ct)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                return ss.Wait(timeout.Value, ct);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        private static async Task<bool> SemaphoreSlimWaitAsync(
            SemaphoreSlim ss,
            TimeSpan? timeout,
            CancellationToken ct)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                return await ss.WaitAsync(timeout.Value, ct);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }

        [UnsupportedOSPlatform("browser")]
        public static bool SemaphoreSlimGuard(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            if (!SemaphoreSlimWait(ss, timeout, ct))
            {
                return false;
            }

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                a(ct);
                return true;
            }
            finally
            {
                ss.Release();
            }
        }

        [UnsupportedOSPlatform("browser")]
        public static (bool Enterted, T Result) SemaphoreSlimGuard<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            if (!SemaphoreSlimWait(ss, timeout, ct))
            {
                return (false, default);
            }

            if (ct.IsCancellationRequested)
            {
                return (false, default);
            }

            try
            {
                return (true, f(ct));
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<bool> SemaphoreSlimGuardAsync(
            SemaphoreSlim ss,
            Action<CancellationToken> a,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            if (!await SemaphoreSlimWaitAsync(ss, timeout, ct))
            {
                return false;
            }

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                a(ct);
                return true;
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, T> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            if (!await SemaphoreSlimWaitAsync(ss, timeout, ct))
            {
                return (false, default);
            }

            if (ct.IsCancellationRequested)
            {
                return (false, default);
            }

            try
            {
                return (true, f(ct));
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<(bool Enterted, T Result)> SemaphoreSlimGuardAsync<T>(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<T>> f,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            if (!await SemaphoreSlimWaitAsync(ss, timeout, ct))
            {
                return (false, default);
            }

            if (ct.IsCancellationRequested)
            {
                return (false, default);
            }

            try
            {
                return (true, await f(ct));
            }
            finally
            {
                ss.Release();
            }
        }

        public static async Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, bool> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = await SemaphoreSlimGuardAsync(ss, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }

        public static async Task<bool> SemaphoreSlimSpinWaitAsync(
            SemaphoreSlim ss,
            Func<CancellationToken, Task<bool>> pred,
            TimeSpan? guardTimeout = null,
            TimeSpan? spinWaitTimeout = null,
            CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = await SemaphoreSlimGuardAsync(ss, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }
        #endregion

        #region Thread
        public static bool ThreadJoin(Thread t, TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;
            return t.Join(timeout.Value);
        }
        #endregion

        #region ManualResetEventSlim
        [UnsupportedOSPlatform("browser")]
        public static bool ManualResetEventSlimWait(
            ManualResetEventSlim mres,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                return mres.Wait(timeout.Value, ct);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
        #endregion

        #region CountdownEvent
        [UnsupportedOSPlatform("browser")]
        public static bool CountdownEventWait(
            CountdownEvent ce,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                return ce.Wait(timeout.Value, ct);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
        #endregion

        #region Barrier
        [UnsupportedOSPlatform("browser")]
        public static bool BarrierSignalAndWait(
            Barrier b,
            TimeSpan? timeout = null,
            CancellationToken ct = default)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (ct.IsCancellationRequested)
            {
                return false;
            }

            try
            {
                return b.SignalAndWait(timeout.Value, ct);
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
        #endregion

        #region ReaderWriterLockSlim
        public static bool ReaderWriterLockSlimReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterReadLock(timeout.Value))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public static (bool Entered, T Result) ReaderWriterLockSlimReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterReadLock(timeout.Value))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                rwls.ExitReadLock();
            }
        }

        public static async Task<bool> ReaderWriterLockSlimReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = ReaderWriterLockSlimReadGuard(rwls, pred, guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }

        public static bool ReaderWriterLockSlimUpgradeableReadGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterUpgradeableReadLock(timeout.Value))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                rwls.ExitUpgradeableReadLock();
            }
        }

        public static (bool Entered, T Result) ReaderWriterLockSlimUpgradeableReadGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterUpgradeableReadLock(timeout.Value))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                rwls.ExitUpgradeableReadLock();
            }
        }

        public static async Task<bool> ReaderWriterLockSlimUpgradeableReadSpinWaitAsync(
             ReaderWriterLockSlim rwls,
             Func<bool> pred,
             TimeSpan? guardTimeout = null,
             TimeSpan? spinWaitTimeout = null,
             CancellationToken ct = default)
        {
            while (true)
            {
                if (ct.IsCancellationRequested)
                {
                    return false;
                }

                (var entered, var result) = ReaderWriterLockSlimUpgradeableReadGuard(
                    rwls,
                    pred,
                    guardTimeout);
                if (entered && result)
                {
                    return true;
                }

                if (!await SpinWaiter(spinWaitTimeout, ct))
                {
                    return false;
                }
            }
        }

        public static bool ReaderWriterLockSlimWriteGuard(
            ReaderWriterLockSlim rwls,
            Action a,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterWriteLock(timeout.Value))
            {
                return false;
            }

            try
            {
                a();
                return true;
            }
            finally
            {
                rwls.ExitWriteLock();
            }
        }

        public static (bool Entered, T Result) ReaderWriterLockSlimWriteGuard<T>(
            ReaderWriterLockSlim rwls,
            Func<T> f,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeoutDefaults.Infinity;

            if (!rwls.TryEnterWriteLock(timeout.Value))
            {
                return (false, default);
            }

            try
            {
                return (true, f());
            }
            finally
            {
                rwls.ExitWriteLock();
            }
        }
        #endregion
    }
}
