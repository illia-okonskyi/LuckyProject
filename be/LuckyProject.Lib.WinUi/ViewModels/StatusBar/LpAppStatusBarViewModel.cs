using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.Basics.Services;
using LuckyProject.Lib.WinUi.Extensions;
using LuckyProject.Lib.WinUi.Models;
using LuckyProject.Lib.WinUi.Services;
using Microsoft.Extensions.Options;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LuckyProject.Lib.WinUi.ViewModels.StatusBar
{
    #region Localization
    public partial class LpAppStatusBarLocalizationViewModel
        : AbstractLpLocalizationViewModel
    {
        #region ctor & props
        public LpAppStatusBarLocalizationViewModel(ILpDesktopAppLocalizationService service)
            : base(service)
        {
            SetProps(GetRegistrations());
        }

        private List<PropRegistration> GetRegistrations()
        {
            var selfType = GetType();
            return new List<PropRegistration>
            {
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.pendingTaskCount",
                    DefaultValue = "Pending task(s)",
                    Property = selfType.GetProperty(nameof(PendingTaskCount))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.pendingTasks",
                    DefaultValue = "Pending tasks",
                    Property = selfType.GetProperty(nameof(PendingTasks))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.errorCount",
                    DefaultValue = "Error(s)",
                    Property = selfType.GetProperty(nameof(ErrorCount))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.warningCount",
                    DefaultValue = "Warning(s)",
                    Property = selfType.GetProperty(nameof(WarningCount))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.messageCount",
                    DefaultValue = "Message(s)",
                    Property = selfType.GetProperty(nameof(MessageCount))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.progress",
                    DefaultValue = "Progress",
                    Property = selfType.GetProperty(nameof(Progress))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.category",
                    DefaultValue = "Category",
                    Property = selfType.GetProperty(nameof(Category))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.description",
                    DefaultValue = "Description",
                    Property = selfType.GetProperty(nameof(Description))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.status",
                    DefaultValue = "Status",
                    Property = selfType.GetProperty(nameof(Status))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.messages",
                    DefaultValue = "Messages",
                    Property = selfType.GetProperty(nameof(Messages))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.time",
                    DefaultValue = "Time",
                    Property = selfType.GetProperty(nameof(Time))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.message",
                    DefaultValue = "Message",
                    Property = selfType.GetProperty(nameof(Message))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.common.strings.cancel",
                    DefaultValue = "Cancel",
                    Property = selfType.GetProperty(nameof(Cancel))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.common.strings.cancelling",
                    DefaultValue = "Cancelling...",
                    Property = selfType.GetProperty(nameof(Cancelling))
                },
                new()
                {
                    Type = LpLocalizationResourceType.String,
                    Key = "lp.lib.winui.statusbar.strings.errorInPendingProcess",
                    DefaultValue = "Error in pending process {0} -> {1}: {2}",
                    Property = selfType.GetProperty(nameof(ErrorInPendingProcess))
                },
             };
        }
        #endregion

        #region Observables
        [ObservableProperty]
        private string pendingTaskCount;

        [ObservableProperty]
        private string pendingTasks;

        [ObservableProperty]
        private string errorCount;

        [ObservableProperty]
        private string warningCount;

        [ObservableProperty]
        private string messageCount;

        [ObservableProperty]
        private string progress;

        [ObservableProperty]
        private string category;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string status;

        [ObservableProperty]
        private string messages;

        [ObservableProperty]
        private string time;

        [ObservableProperty]
        private string message;

        [ObservableProperty]
        private string cancel;

        [ObservableProperty]
        private string cancelling;

        [ObservableProperty]
        private string errorInPendingProcess;
        #endregion
    }
    #endregion

    #region ViewModel
    public partial class LpAppStatusBarViewModel
        : LpViewModel<LpAppStatusBarLocalizationViewModel>
        , ILpAppStatusBarService
    {
        #region Internals & ctor
        private readonly LpAppStatusBarOptions options;
        private readonly IThreadSyncService tsService;
        private readonly object lockPendingTasks = new();
        private readonly ICommand cancelTaskCommand;
        private readonly List<LpAppMessage> messages = new();
        private bool isCancellingAllCancelableTasks;

        public LpAppStatusBarViewModel(
            IOptions<LpAppStatusBarOptions> options,
            IThreadSyncService tsService)
        {
            this.options = options.Value;
            this.tsService = tsService;
            cancelTaskCommand = new RelayCommand<LpAppTaskViewModel>(
                async t => await t.CancelTaskAsync());
            CancelAllCancelableTasksCommand = new(
                CancelAllCancelableTasksAsync,
                () => !isCancellingAllCancelableTasks);
            ClearMessagesCommand = new RelayCommand(ClearMessages);
        }
        #endregion

        #region Public & observables
        [ObservableProperty]
        private bool isExpanded;
        [ObservableProperty]
        private double savedExpandedHeight;
        public double CollapsedHeight => options.CollapsedHeight;

        [ObservableProperty]
        private ObservableCollection<LpAppTaskViewModel> pendingTasks = new();
        [ObservableProperty]
        private int totalPendingTasksCount;
        [ObservableProperty]
        private int cancelablePendingTasksCount;

        [ObservableProperty]
        private bool isBusy;
        public RelayCommand CancelAllCancelableTasksCommand { get; }

        [ObservableProperty]
        private ObservableCollection<LpAppMessage> filteredMessages = new();
        [ObservableProperty]
        private int infoMessagesCount;
        [ObservableProperty]
        private int warningMessagesCount;
        [ObservableProperty]
        private int errorMessagesCount;
        [ObservableProperty]
        private bool showInfoMessages = true;
        [ObservableProperty]
        private bool showWarningMessages = true;
        [ObservableProperty]
        private bool showErrorMessages = true;
        public ICommand ClearMessagesCommand { get; }
        #endregion

        #region Interface impl
        #region CreatePendingTask
        public (Task<SafeAwaitResult> Task, Func<Task> CancelTaskAsync) CreatePendingTask(
            string category,
            string description,
            string status,
            Func<LpAppTaskContext, CancellationToken, Task> handler,
            bool isCancelable = false,
            LpAppMessageType errorMessageType = LpAppMessageType.Error)
        {
            var taskVm = new LpAppTaskViewModel()
            {
                Category = category,
                Description = description,
                Status = status,
                CancelString = Localization.Cancel,
                CancellingString = Localization.Cancelling,
                CancelCommand = cancelTaskCommand,
            };
            CancellationTokenSource cts = null;
            if (isCancelable)
            {
                cts = new CancellationTokenSource();
                taskVm.CancelTaskAsync = async () =>
                {
                    taskVm.IsCancelButtonVisible = false;
                    taskVm.IsCancelling = true;
                    cts.Cancel();
                    await taskVm.Task.SafeAwaitWrapper();
                };
                taskVm.IsCancelButtonVisible = true;
            }

            var appRoot = Application.Current.GetLpApplicationRoot();
            var context = new LpAppTaskContext()
            {
                SetStatus = s => appRoot.TryEnqueueToDispatcherQueue(() => taskVm.Status = s),
                SetProgressValue = v => appRoot.TryEnqueueToDispatcherQueue(
                    () => taskVm.Progress.Value = v),
                SetProgressIndeterminate = i => appRoot.TryEnqueueToDispatcherQueue(
                    () => taskVm.Progress.IsIndeterminate = i)
            };
            var handlerWrapper = async () =>
            {
                var r = await PendingTaskHandlerSafeAwait(
                    taskVm,
                    handler(context, cts?.Token ?? default),
                    category,
                    description,
                    errorMessageType);
                OnPendingTaskCompleted(taskVm);
                return r;
            };
            AddPendingTask(taskVm);
            taskVm.Task = handlerWrapper();
            return (taskVm.Task, taskVm.CancelTaskAsync);
        }
        #endregion

        #region AddMessage
        public void AddMessage(LpAppMessage message)
        {
            Application.Current.GetLpApplicationRoot().TryEnqueueToDispatcherQueue(() =>
            {
                if (messages.Count == options.MaxMessagesCount)
                {
                    var removedType = messages[0].Type;
                    messages.RemoveAt(messages.Count - 1);
                }

                messages.Add(message);
                UpdateFilteredMessages();
            });
        }
        #endregion

        #region ExceptionGuards
        public bool ExceptionGuard(
            Action action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error)
        {
            try
            {
                action();
                return true;
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex, type, category, errorMessageFormat);
                return false;
            }
        }
        public (bool Success, T Value) ExceptionGuard<T>(
            Func<T> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error)
        {
            try
            {
                return (true, action());
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex, type, category, errorMessageFormat);
                return (false, default);
            }
        }

        public async Task<SafeAwaitResult> ExceptionGuardAsync(
            Func<Task> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error)
        {
            try
            {
                await action();
                return SafeAwaitResult.Success;
            }
            catch (OperationCanceledException)
            {
                return SafeAwaitResult.Cancelled;
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex, type, category, errorMessageFormat);
                return SafeAwaitResult.Exception;
            }
        }
        
        public async Task<(SafeAwaitResult Result, T Value)> ExceptionGuardAsync<T>(
            Func<Task<T>> action,
            string category = null,
            string errorMessageFormat = null,
            LpAppMessageType type = LpAppMessageType.Error)
        {
            try
            {
                return (SafeAwaitResult.Success, await action());
            }
            catch (OperationCanceledException)
            {
                return (SafeAwaitResult.Cancelled, default);
            }
            catch (Exception ex)
            {
                AddExceptionMessage(ex, type, category, errorMessageFormat);
                return (SafeAwaitResult.Exception, default);
            }
        }
        #endregion
        #endregion

        #region Internals
        private void AddPendingTask(LpAppTaskViewModel taskVm)
        {
            tsService.MonitorGuard(lockPendingTasks, () =>
            {
                PendingTasks.Add(taskVm);
                ++TotalPendingTasksCount;
                IsBusy = true;
                if (taskVm.IsCancelable)
                {
                    ++CancelablePendingTasksCount;
                    CancelAllCancelableTasksCommand.NotifyCanExecuteChanged();
                }
            });
        }

        private void OnPendingTaskCompleted(LpAppTaskViewModel taskVm)
        {
            tsService.MonitorGuard(lockPendingTasks, () =>
            {
                PendingTasks.Remove(taskVm);
                --TotalPendingTasksCount;
                IsBusy = TotalPendingTasksCount > 0;
                if (taskVm.IsCancelable)
                {
                    --CancelablePendingTasksCount;
                    CancelAllCancelableTasksCommand.NotifyCanExecuteChanged();
                }
            });
        }

        private async void CancelAllCancelableTasksAsync()
        {
            var taskVms = PendingTasks.Where(t => t.IsCancelable).ToList();
            var tasks = taskVms.Select(vm => vm.CancelTaskAsync().SafeAwaitWrapper()).ToList();
            isCancellingAllCancelableTasks = true;
            CancelAllCancelableTasksCommand.NotifyCanExecuteChanged();
            await Task.WhenAll(tasks).SafeAwaitWrapper();
            isCancellingAllCancelableTasks = false;
            CancelAllCancelableTasksCommand.NotifyCanExecuteChanged();
        }

        private void ClearMessages()
        {
            messages.Clear();
            FilteredMessages.Clear();
            InfoMessagesCount = 0;
            WarningMessagesCount = 0;
            ErrorMessagesCount = 0;
        }

        private void UpdateFilteredMessages()
        {
            var types = new HashSet<LpAppMessageType>();
            if (ShowInfoMessages)
            {
                types.Add(LpAppMessageType.Info);
            }
            if (ShowWarningMessages)
            {
                types.Add(LpAppMessageType.Warning);
            }
            if (ShowErrorMessages)
            {
                types.Add(LpAppMessageType.Error);
            }

            if (types.Count == 0)
            {
                FilteredMessages.Clear();
                return;
            }

            var infoMessagesCount = 0;
            var warningMessagesCount = 0;
            var errorMessagesCount = 0;
            messages.ForEach(m =>
            {
                if (m.Type == LpAppMessageType.Info)
                {
                    infoMessagesCount++;
                }
                else if (m.Type == LpAppMessageType.Warning)
                {
                    warningMessagesCount++;
                }
                else
                {
                    errorMessagesCount++;
                }
            });

            var filtered = messages.Where(m => types.Contains(m.Type)).Reverse().ToList();
            var result = new ObservableCollection<LpAppMessage>();
            filtered.ForEach(result.Add);
            FilteredMessages = result;
            InfoMessagesCount = infoMessagesCount;
            WarningMessagesCount = warningMessagesCount;
            ErrorMessagesCount = errorMessagesCount;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.PropertyName == nameof(ShowInfoMessages) ||
                e.PropertyName == nameof(ShowWarningMessages) ||
                e.PropertyName == nameof(ShowErrorMessages))
            {
                UpdateFilteredMessages();
            }
        }

        private void AddExceptionMessage(
            Exception ex,
            LpAppMessageType type,
            string category,
            string errorMessageFormat)
        {
            var message = string.IsNullOrEmpty(errorMessageFormat)
                ? ex.Message
                : string.Format(errorMessageFormat, ex.Message);
            AddMessage(new()
            {
                Type = type,
                Category = category,
                Message = message
            });
        }

        private async Task<SafeAwaitResult> PendingTaskHandlerSafeAwait(
            LpAppTaskViewModel taskVm,
            Task handler,
            string category,
            string description,
            LpAppMessageType errorMessageType)
        {
            try
            {
                await handler;
                return SafeAwaitResult.Success;
            }
            catch (OperationCanceledException)
            {
                return SafeAwaitResult.Cancelled;
            }
            catch (Exception ex)
            {
                AddMessage(new()
                {
                    Type = errorMessageType,
                    Category = category,
                    Message = string.Format(
                        Localization.ErrorInPendingProcess,
                        description,
                        taskVm.Status,
                        ex.Message)
                });
                return SafeAwaitResult.Exception;
            }
        }
        #endregion
    }
    #endregion
}
