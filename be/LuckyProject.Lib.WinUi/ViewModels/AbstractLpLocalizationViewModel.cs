using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Models.Localization;
using LuckyProject.Lib.WinUi.Services;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    public abstract partial class AbstractLpLocalizationViewModel
         : ObservableObject
         , ILpDesktopAppLocalizationConsumer
    {
        #region PropRegistration
        public class PropRegistration
        {
            public string Key { get; init; }
            public LpLocalizationResourceType Type { get; init; }
            public PropertyInfo Property { get; init; }
            public string DefaultValue { get; init; }
        }
        #endregion

        #region Internals & ctor & Dispose
        private List<PropRegistration> props = new();
        protected AbstractLpLocalizationViewModel(ILpDesktopAppLocalizationService service)
        {
            Service = service;
            service.AddConsumer(this);
        }

        public void Dispose()
        {
            Service.DeleteConsumer(this);
        }
        #endregion

        #region Public
        public ILpDesktopAppLocalizationService Service { get; }
        #endregion

        #region Protected & Interface impl
        protected void SetProps(List<PropRegistration> props)
        {
            this.props = props;
            Update();
        }

        protected void Update()
        {
            var i18nStrings = Service.GetLocalizedStrings(this.props
                .Where(p => p.Type == LpLocalizationResourceType.String)
                .ToDictionary(p => p.Key, p => p.DefaultValue));
            var i18nPaths = Service.GetLocalizedFilePaths(this.props
                .Where(p => p.Type == LpLocalizationResourceType.File)
                .ToDictionary(p => p.Key, p => p.DefaultValue));
            var props = this.props.ToDictionary(p => p.Key, p => p.Property);
            ApplyLocalization(props, i18nStrings);
            ApplyLocalization(props, i18nPaths);
        }

        public virtual void OnLocalizationUpdated(ILpDesktopAppLocalizationService service)
        {
            Update();
        }
        #endregion

        #region Internals
        private void ApplyLocalization(
            Dictionary<string, PropertyInfo> props,
            Dictionary<string, string> localization)
        {
            if (props == null)
            {
                return;
            }

            if (localization.Count == 0)
            {
                return;
            }

            foreach (var kvp in props)
            {
                var key = kvp.Key;
                var propInfo = kvp.Value;
                var value = localization[key];
                propInfo.SetValue(this, value);
            }
        }
        #endregion
    }
}
