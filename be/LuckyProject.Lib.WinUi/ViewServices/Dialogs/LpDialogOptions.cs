using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    
    public class LpDialogOptions
    {
        public XamlRoot XamlRool { get; set; }
        public string Title { get; set; }
        public LpDialogButtonOptions PrimaryButton { get; set; }
        public LpDialogButtonOptions SecondaryButton { get; set; }
        public LpDialogButtonOptions CloseButton { get; set; }
        public LpDialogClosePolicy ClosePolicy { get; set; } = LpDialogClosePolicy.Any;
        public Func<object, Task<bool>> CancelHandler { get; set; }
        public object CancelHandlerContext { get; set; }
        public ContentDialogButton DefaultButton { get; set; } = ContentDialogButton.Primary;
        public object Content { get; set; }
        public HorizontalAlignment CommandSpaceAlignment { get; set; } = HorizontalAlignment.Right;
        public LpDialogBlockOptions Block { get; set; }

    }
}
