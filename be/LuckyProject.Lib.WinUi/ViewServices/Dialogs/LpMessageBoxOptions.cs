using Microsoft.UI.Xaml;

namespace LuckyProject.Lib.WinUi.ViewServices.Dialogs
{
    public class LpMessageBoxOptions
    {
        public XamlRoot XamlRool { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public LpMessageBoxIcon Icon { get; set; } = LpMessageBoxIcon.None;
        public LpMessageBoxButtons Buttons { get; set; } = LpMessageBoxButtons.Ok;
        public string CustomDefaultButton { get; set; }
        public string CustomCancelButton { get; set; }
        public string CustomSecondaryButton { get; set; }
        public object ExtraContent { get; set; }
        public LpDialogBlockOptions Block { get; set; }

        public bool SingleCustomButton =>
            !string.IsNullOrEmpty(CustomDefaultButton) &&
            string.IsNullOrEmpty(CustomCancelButton) &&
            string.IsNullOrEmpty(CustomSecondaryButton);
        public bool TwoCustomButtons =>
            !string.IsNullOrEmpty(CustomDefaultButton) &&
            !string.IsNullOrEmpty(CustomCancelButton) &&
            string.IsNullOrEmpty(CustomSecondaryButton);
        public bool ThreeCustomButtons =>
            !string.IsNullOrEmpty(CustomDefaultButton) &&
            !string.IsNullOrEmpty(CustomCancelButton) &&
            !string.IsNullOrEmpty(CustomSecondaryButton);
    }
}
