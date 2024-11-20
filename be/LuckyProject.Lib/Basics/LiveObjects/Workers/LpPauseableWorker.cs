using LuckyProject.Lib.Basics.LiveObjects.Workers.Payloads;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPauseableWorker<TPayload>
        : AbstractLpPauseableWorkerBase,
        ILpPauseableWorker<TPayload>
        where TPayload : ILpWorkerPayload
    {
        #region Public properties
        public TPayload Payload { get; }
        #endregion

        #region ctor & Dispose
        public LpPauseableWorker(
            TPayload payload,
            string name = null,
            ILogger logger = null)
            : base(name, logger)
        {
            Payload = payload ?? throw new ArgumentNullException(nameof(payload));
        }

        public override void Dispose()
        {
            Payload.Dispose();
            base.Dispose();
        }
        #endregion

        #region Internals
        protected override Task<int> PauseableWorkerAsync(
            int startingProgress,
            CancellationToken cancellationToken) => 
            Payload.PauseableWorkerPayloadAsync(
                this,
                new()
                {
                    StartingProgress = startingProgress,
                    CancellationToken = cancellationToken,
                    GetCancelSource = GetCancelSource,
                    SetProgress = SetProgress
                });
        #endregion
    }
}
