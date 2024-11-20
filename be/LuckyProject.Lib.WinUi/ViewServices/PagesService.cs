using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckyProject.Lib.WinUi.ViewServices
{
    public class PagesService : IPagesService
    {
        #region Internals & ctor & Dispose
        private readonly List<IPagesService.PageRegistration> pages = new();
        private readonly Dictionary<string, Type> pageMappings = new();
        #endregion

        #region Interface impl
        public void RegisterPages(List<IPagesService.PageRegistration> registrations)
        {
            pages.AddRange(registrations);
            registrations.ForEach(r => pageMappings.Add(r.Key, r.ViewType));
        }

        public List<IPagesService.PageRegistration> GetPages() => pages.ToList();

        public Type GetPageType(string key) =>
            pageMappings.TryGetValue(key, out var pageType) ? pageType : null;

        public Type GetSettingsPageType()
        {
            var r = pages.FirstOrDefault(p => p.IsSettingsPage);
            if (r == null)
            {
                throw new InvalidOperationException("Settings page registration not found");
            }
            return pageMappings[r.Key];
        }
        #endregion
    }
}