using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.WinUi.ViewServices
{
    public interface IPagesService
    {
        #region PageRegistration
        public enum PageType
        {
            Default,
            Starting,
            Settings,
        }

        public class PageRegistration
        {
            public PageType Type { get; init; } = PageType.Default;
            public Type ViewModelType { get; init; }
            public Type ViewType { get; init; }
            public string Key => ViewModelType.FullName;
            public bool IsSettingsPage => Type == PageType.Settings;
            public bool IsStartingPage => Type == PageType.Starting;
        }
        #endregion

        void RegisterPages(List<PageRegistration> registrations);
        List<PageRegistration> GetPages();
        Type GetPageType(string key);
        Type GetSettingsPageType();
    }
}
