using CommunityToolkit.Mvvm.ComponentModel;
using FluentValidation;
using LuckyProject.Lib.Basics.Helpers;
using LuckyProject.Lib.Basics.Services;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using System.Threading;
using LuckyProject.Lib.Basics.Exceptions;

namespace LuckyProject.Lib.WinUi.ViewModels
{
    #region LpViewModel
    public class LpViewModel : ObservableObject
    { }

    public class LpViewModel<TLocalizationViewModel> : LpViewModel , IDisposable
        where TLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        public LpViewModel(IServiceProvider serviceProvider = null)
        {
            serviceProvider ??= StaticRootServiceProviderContainer
                .RootServiceProvider;
            Localization = serviceProvider.GetRequiredService<TLocalizationViewModel>();
        }

        public TLocalizationViewModel Localization { get; }

        public virtual void Dispose()
        {
            Localization.Dispose();
        }
    }
    #endregion

    #region LpFormViewModel
    public class LpFormViewModel<TLocalizationViewModel> : LpViewModel<TLocalizationViewModel>
        where TLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        private readonly IValidationService validationService;

        public LpFormViewModel(IServiceProvider serviceProvider = null)
            : base(serviceProvider)
        {
            serviceProvider ??= StaticRootServiceProviderContainer
                .RootServiceProvider;
            validationService = serviceProvider.GetRequiredService<IValidationService>();
            Validation = serviceProvider.GetRequiredService<LpValidationViewModel>();
        }

        public LpValidationViewModel Validation { get; }

        protected bool Validate<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null)
        {
            var result = validationService.Validate(value, validator, detailsFormatter);
            Validation.Update(result);
            return result.IsValid;
        }

        protected async Task<bool> ValidateAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default)
        {
            var result = await validationService.ValidateAsync(
                value,
                validator,
                detailsFormatter,
                cancellationToken);
            Validation.Update(result);
            return result.IsValid;
        }

        protected bool ValidationGuard(Action action) => Validation.ValidationGuard(action);
        protected (bool IsValid, T Result) ValidationGuard<T>(Func<T> action) =>
            Validation.ValidationGuard(action);
        protected Task<bool> ValidationGuardAsync(Func<Task> action) =>
            Validation.ValidationGuardAsync(action);
        protected Task<(bool IsValid, T Result)> ValidationGuardAsync<T>(Func<Task<T>> action) =>
            Validation.ValidationGuardAsync(action);
    }
    #endregion

    #region LpRecipientViewModel
    public class LpRecipientViewModel : ObservableRecipient
    { }

    public class LpRecipientViewModel<TLocalizationViewModel> : LpRecipientViewModel, IDisposable
        where TLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        public LpRecipientViewModel(IServiceProvider serviceProvider = null)
        {
            serviceProvider ??= StaticRootServiceProviderContainer
                .RootServiceProvider;
            Localization = serviceProvider.GetRequiredService<TLocalizationViewModel>();
        }

        public TLocalizationViewModel Localization { get; }

        public virtual void Dispose()
        {
            Localization.Dispose();
        }
    }
    #endregion

    #region LpRecipientFormViewModel
    public class LpRecipientFormViewModel<TLocalizationViewModel>
        : LpRecipientViewModel<TLocalizationViewModel>
        where TLocalizationViewModel : AbstractLpLocalizationViewModel
    {
        private readonly IValidationService validationService;

        public LpRecipientFormViewModel(IServiceProvider serviceProvider = null)
            : base(serviceProvider)
        {
            serviceProvider ??= StaticRootServiceProviderContainer
                .RootServiceProvider;
            validationService = serviceProvider.GetRequiredService<IValidationService>();
            Validation = serviceProvider.GetRequiredService<LpValidationViewModel>();
        }

        public LpValidationViewModel Validation { get; }

        protected bool Validate<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null)
        {
            var result = validationService.Validate(value, validator, detailsFormatter);
            Validation.Update(result);
            return result.IsValid;
        }

        protected async Task<bool> ValidateAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default)
        {
            var result = await validationService.ValidateAsync(
                value,
                validator,
                detailsFormatter,
                cancellationToken);
            Validation.Update(result);
            return result.IsValid;
        }
    }
    #endregion
}
