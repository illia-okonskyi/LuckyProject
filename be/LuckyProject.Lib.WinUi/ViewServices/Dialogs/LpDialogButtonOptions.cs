using System;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpDialogButtonOptions
    {
        public string Text { get; set; }
        public Func<object, Task<bool>> ClickHandler { get; set; }
        public object ClickHandlerContext { get; set; }
    }
}
