using FluentValidation;
using LuckyProject.Lib.Basics.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.Lib.Basics.Services
{
    public interface IValidationService
    {
        #region Validate
        LpValidationResult Validate<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null);
        Task<LpValidationResult> ValidateAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default);
        LpValidationResult Validate(ModelStateDictionary msd);
        #endregion

        #region EnsureValid
        void EnsureValid<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null);
        Task EnsureValidAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default);
        void EnsureValid(ModelStateDictionary msd);
        #endregion
    }
}
