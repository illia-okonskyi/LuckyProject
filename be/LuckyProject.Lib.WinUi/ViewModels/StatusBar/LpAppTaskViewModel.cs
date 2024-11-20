using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LuckyProject.Lib.WinUi.ViewModels.StatusBar
{
    public partial class LpAppTaskViewModel : ObservableObject
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Category { get; init; }
        public string Description { get; init; }
        [ObservableProperty]
        private string status;
        public LpProgressViewModel Progress { get; init; } = new();
        public Task<SafeAwaitResult> Task { get; set; }
        public Func<Task> CancelTaskAsync { get; set; }
        public bool IsCancelable => CancelTaskAsync != null;
        public string CancelString { get; init; }
        public string CancellingString { get; init; }
        public ICommand CancelCommand { get; init; }
        [ObservableProperty]
        private bool isCancelButtonVisible;
        [ObservableProperty]
        private bool isCancelling;
    }
}
