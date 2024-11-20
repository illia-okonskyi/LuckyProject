using LuckyProject.Lib.WinUi.Models;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public class LpApplicationCloseOptions
    {
        public Func<LpAppTaskContext, CancellationToken, Task> SaveAppDataAsyncHandler
        {
            get;
            init;
        }
        public bool SaveMainWindowState { get; init; } = true;
        public bool SaveStatusBarState { get; init; } = true;
        public Func<Task<bool>> ExtraCloseAsyncHandler { get; init; }
    }
}
