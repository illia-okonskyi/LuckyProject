using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.EntryPoint
{
    public class LpApplicationInitContext<TSettings>
         where TSettings : class, new()
    {
        public class LocalizationsContext
        {
            public string Loading { get; init; }
            public string Loading1 { get; init; }
            public string Loading2 { get; init; }

            public Func<string, string, string> GetLocalizedString { get; init; }
            public Func<
                Dictionary<string, string>,
                Dictionary<string, string>> GetLocalizedStrings { get; init; }
            public Func<string, string, string> GetLocalizedFilePath { get; init; }
            public Func<
                Dictionary<string, string>,
                Dictionary<string, string>> GetLocalizedFilePaths { get; init; }
        }

        public LocalizationsContext Localizations { get; init; }
        public Action<int> SetInitProgressValue { get; init; }
        public Action<bool> SetInitProgressIsIndeterminate { get; init; }
        public Action<string> SetInitStatus { get; init; }
        public Func<string, Task> LoadLocalizationSourceAsync { get; init; }
        public TSettings CurrentSettings { get; init; }
    }
}
