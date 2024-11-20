namespace LuckyProject.Lib.WinUi.ViewServices.Navigation
{
    public interface INavigationAware
    {
        void OnNavigatedTo(object parameter);
        void OnNavigatedFrom();
    }
}
