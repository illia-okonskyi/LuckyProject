using Microsoft.UI.Xaml;
using System.Threading.Tasks;
using System.Threading;
using System;
using LuckyProject.Lib.WinUi.Models;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpProgressDialogOptions
    {
        public XamlRoot XamlRool { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public object ExtraContent { get; set; }
        public bool IsIndeterminate { get; set; }
        public bool IsCancelable { get; set; }
        public Func<LpAppTaskContext, CancellationToken, Task> Handler { get; set; }
    }
}
