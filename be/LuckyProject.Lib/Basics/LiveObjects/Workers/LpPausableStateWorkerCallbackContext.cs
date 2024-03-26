namespace LuckyProject.Lib.Basics.LiveObjects.Workers
{
    public class LpPausableStateWorkerCallbackContext<TState> : LpPausableWorkerCallbackContext
    {
        public ILpPausableWorker.SetProgressStateFunc<TState> SetProgressState { get; init; }
        public TState StartingState { get; init; }
    }
}
