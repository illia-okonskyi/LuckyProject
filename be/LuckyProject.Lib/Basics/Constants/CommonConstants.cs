namespace LuckyProject.Lib.Basics.Constants
{
    public static class CommonConstants
    {
        public const int ProgressMin = 0;
        public const int ProgressMax = 100;

        public static bool ProgressInRange(int progress)
        {
            return progress >= ProgressMin && progress <= ProgressMax;
        }
    }
}
