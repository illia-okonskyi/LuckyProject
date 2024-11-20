using CommunityToolkit.Mvvm.ComponentModel;
using LuckyProject.Lib.Basics.Exceptions;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.WinUi.Services;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    #region LpValidationFieldViewModel
    public partial class LpValidationErrorViewModel : ObservableObject
    {
        #region Internals & ctor
        private readonly ILpDesktopAppLocalizationService localizationService;
        private readonly string validationPath;
        private readonly Dictionary<string, string> customMessagesLocalizationKeysAndDefaults;

        public LpValidationErrorViewModel(
            ILpDesktopAppLocalizationService localizationService,
            string validationPath,
            string displayName,
            int order = 0,
            Dictionary<string, string> customMessagesLocalizationKeysAndDefaults = null)
        {
            this.localizationService = localizationService;
            this.validationPath = validationPath;
            this.customMessagesLocalizationKeysAndDefaults =
                customMessagesLocalizationKeysAndDefaults;
            DisplayName = displayName;
            SummaryOrder = order;
        }
        #endregion

        #region Public & Observables
        public string DisplayName { get; }
        public int SummaryOrder { get; }
        public List<string> Errors { get; private set; }

        [ObservableProperty]
        private bool isValid = true;
        [ObservableProperty]
        private Visibility visibility = Visibility.Collapsed;
        [ObservableProperty]
        private string error;
        #endregion

        #region Public interface
        public void Reset()
        {
            IsValid = true;
            Visibility = Visibility.Collapsed;
            Error = null;
        }

        public void Update(LpValidationResult validationResult)
        {
            Reset();
            if (validationResult.IsValid)
            {
                return;
            }

            var validationError = validationResult
                .Errors
                .FirstOrDefault(e => e.Path == validationPath);
            if (validationError == null)
            {
                return;
            }

            IsValid = false;
            Visibility = Visibility.Visible;
            Errors = GetErrorMessages(validationError);
            Error = BuildErrorMessage();
        }
        #endregion

        #region Internals
        private string BuildErrorMessage()
        {
            if (Errors.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var error in Errors.SkipLast(1))
            {
                sb.AppendLine($"- {error}");
            }
            sb.Append($"- {Errors.Last()}");
            return sb.ToString();
        }

        private List<string> GetErrorMessages(LpValidationError error)
        {
            var isCustomMessages = customMessagesLocalizationKeysAndDefaults != null;
            if (isCustomMessages)
            {
                return localizationService
                    .GetLocalizedStrings(customMessagesLocalizationKeysAndDefaults)
                    .Select(kvp => kvp.Value)
                    .ToList();
            }

            return localizationService.ValidationLocalization.GetMessages(error);
        }
        #endregion
    }
    #endregion

    #region LpValidationSummaryViewModel
    public partial class LpValidationSummaryViewModel : ObservableObject
    {
        [ObservableProperty]
        private Visibility visibility = Visibility.Collapsed;
        [ObservableProperty]
        private string errors;
    }
    #endregion

    #region LpValidationViewModel
    public partial class LpValidationViewModel : ObservableObject
    {
        #region FieldRegistration
        public class FieldRegistration
        {
            public string ValidationPath { get; init; }
            public string DisplayName { get; init; }
            public int SummaryOrder { get; init; }
            public Dictionary<string, string> CustomErrorMessagesLocalizationKeysAndDefaults
            {
                get;
                init;
            }
        }
        #endregion

        #region Internals & ctor
        private readonly ILpDesktopAppLocalizationService localizationService;

        public LpValidationViewModel(ILpDesktopAppLocalizationService localizationService)
        {
            this.localizationService = localizationService;
        }
        #endregion

        #region Public & Observables
        [ObservableProperty]
        private bool isValid = true;

        public Dictionary<string, LpValidationErrorViewModel> Fields { get; } = new();
        public LpValidationSummaryViewModel Summary { get; } = new();

        #endregion

        #region Public Interface
        public void RegisterFields(List<FieldRegistration> fields)
        {
            fields.ForEach(f => Fields.Add(
                f.ValidationPath,
                new LpValidationErrorViewModel(
                    localizationService,
                    f.ValidationPath,
                    f.DisplayName,
                    f.SummaryOrder,
                    f.CustomErrorMessagesLocalizationKeysAndDefaults)));
            
        }

        public void Reset()
        {
            IsValid = true;
            Summary.Visibility = Visibility.Collapsed;
            Summary.Errors = null;
            foreach (var field in Fields.Values)
            {
                field.Reset();
            }
        }

        public void Update(LpValidationResult validationResult)
        {
            IsValid = validationResult.IsValid;
            if (IsValid)
            {
                Reset();
                return;
            }

            foreach (var field in Fields.Values)
            {
                field.Update(validationResult);
            }

            Summary.Errors = BuildSummaryErrors();
            Summary.Visibility = Visibility.Visible;
        }

        public bool ValidationGuard(Action action)
        {
            try
            {
                Reset();
                action();
                return true;
            }
            catch (LpValidationErrorException ex)
            {
                Update(ex.Result);
                return false;
            }
        }

        public (bool IsValid, T Result) ValidationGuard<T>(Func<T> action)
        {
            try
            {
                Reset();
                return (true, action());
            }
            catch (LpValidationErrorException ex)
            {
                Update(ex.Result);
                return (false, default);
            }
        }

        public async Task<bool> ValidationGuardAsync(Func<Task> action)
        {
            try
            {
                Reset();
                await action();
                return true;
            }
            catch (LpValidationErrorException ex)
            {
                Update(ex.Result);
                return false;
            }
        }

        public async Task<(bool IsValid, T Result)> ValidationGuardAsync<T>(Func<Task<T>> action)
        {
            try
            {
                Reset();
                return (true, await action());
            }
            catch (LpValidationErrorException ex)
            {
                Update(ex.Result);
                return (false, default);
            }
        }
        #endregion

        #region Internals
        private string BuildSummaryErrors()
        {
            var fieldsAndErrors = Fields
                 .Values
                 .Where(f => !f.IsValid)
                 .OrderBy(f => f.SummaryOrder)
                 .Select(f => new { f.DisplayName, f.Errors })
                 .ToList();
            var sb = new StringBuilder();
            foreach (var f in fieldsAndErrors.SkipLast(1))
            {
                sb.AppendLine($"- {f.DisplayName}:");
                f.Errors.ForEach(e => sb.AppendLine($"    - {e}"));
            }
            var lastField = fieldsAndErrors.Last();
            sb.AppendLine($"- {lastField.DisplayName}:");
            foreach (var e in lastField.Errors.SkipLast(1))
            {
                sb.AppendLine($"    - {e}");
            }
            sb.Append($"    - {lastField.Errors.Last()}");
            return sb.ToString();
        }
        #endregion
    }
    #endregion
}
