namespace LuckyProject.Lib.WinUi.ViewServices
{
    public interface ITaskbarService
    {
        void SetState(TaskbarState taskbarState);
        void SetValue(double progressValue, double progressMax);
    }

}
