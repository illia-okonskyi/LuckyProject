using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Processes
{
    public class LpConsoleProcessExecParams
    {
        /// <summary>
        /// Must return when to post sigkill
        /// </summary>
        public Func<CancellationToken, Task> SigKillCallback { get; init; }

        /// <summary>
        /// Must async return next string for writing[line] to stdin or null/empty string if no
        /// more writes needed
        /// </summary>
        public Func<CancellationToken, Task<LpStringModePair>> StdInCallback { get; init; }
        /// <summary>
        /// Must async return true if callback wants to receive next stdout data chunk
        /// </summary>
        public Func<LpStringModePair, CancellationToken, Task<LpStringModePair>> StdOutCallback
        {
            get;
            init;
        }
        public LpStdStreamReadWriteMode StdOutDefaultReadMode { get; init; } =
            LpStdStreamReadWriteMode.Chunk;
        /// <summary>
        /// Must async return true if callback wants to receive next stderr data chunk
        /// </summary>
        public Func<LpStringModePair, CancellationToken, Task<LpStringModePair>> StdErrCallback
        {
            get;
            init;
        }
        public LpStdStreamReadWriteMode StdErrDefaultReadMode { get; init; } =
            LpStdStreamReadWriteMode.Chunk;
    }
}
