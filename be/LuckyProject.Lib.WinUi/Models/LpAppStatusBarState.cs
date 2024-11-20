namespace LuckyProject.Lib.WinUi.Models
{
    public class LpAppStatusBarState
    {
        public static readonly LpAppStatusBarState Default = new()
        {
            IsExpanded = false,
            SavedExpandedHeight = 200
        };

        public bool IsExpanded { get; set; }
        public double SavedExpandedHeight { get; set; }
    }
}
