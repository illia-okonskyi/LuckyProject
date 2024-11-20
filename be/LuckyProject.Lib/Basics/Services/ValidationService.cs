using FluentValidation.Results;
using FluentValidation;
using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Models;
using LuckyProject.Lib.Basics.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace LuckyProject.Lib.Basics.Services
{
    public class ValidationService : IValidationService
    {
        #region Internals
        private static readonly ILpValidationDetailsFormatter DefaultDetailsFormatter =
            new LpValidationDetailsFormattter();
        private readonly IStringService stringService;
        #endregion

        #region ctor
        public ValidationService(IStringService stringService)
        {
            this.stringService = stringService;
        }
        #endregion

        #region Validate
        public LpValidationResult Validate<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null)
        {
            return CreateValidationResult(validator.Validate(value), detailsFormatter);
        }

        public async Task<LpValidationResult> ValidateAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var backendResult = await validator.ValidateAsync(value, cancellationToken);
                return CreateValidationResult(backendResult, detailsFormatter);
            }
            catch (OperationCanceledException)
            {
                return new LpValidationResult() { IsCancelled = true };
            }
        }

        public LpValidationResult Validate(ModelStateDictionary msd)
        {
            if (msd.IsValid)
            {
                return new LpValidationResult();
            }

            return new LpValidationResult()
            {
                Errors = msd
                    .Select(ms => new LpValidationError(
                        ms.Key,
                        ms.Value.Errors.Select(e => e.ErrorMessage).ToList()))
                    .ToList()
            };
        }
        #endregion

        #region EnsureValid
        public void EnsureValid<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null)
        {
            var result = Validate(value, validator, detailsFormatter);
            if (result.IsValid)
            {
                return;
            }

            throw new LpValidationErrorException(result);
        }

        public async Task EnsureValidAsync<T>(
            T value,
            IValidator<T> validator,
            ILpValidationDetailsFormatter detailsFormatter = null,
            CancellationToken cancellationToken = default)
        {
            var result = await ValidateAsync(value, validator, detailsFormatter, cancellationToken);
            if (result.IsValid)
            {
                return;
            }

            throw new LpValidationErrorException(result);
        }

        public void EnsureValid(ModelStateDictionary msd)
        {
            var result = Validate(msd);
            if (result.IsValid)
            {
                return;
            }

            throw new LpValidationErrorException(result);
        }
        #endregion

        #region Internals
        private LpValidationResult CreateValidationResult(
            ValidationResult backend,
            ILpValidationDetailsFormatter detailsFormatter)
        {
            var result = new LpValidationResult();
            if (backend.IsValid)
            {
                return result;
            }

            detailsFormatter ??= DefaultDetailsFormatter;
            result.Errors.AddRange(backend
                .Errors
                .Select(e => CreateValidationError(e, detailsFormatter)));
            return result;
        }

        private LpValidationError CreateValidationError(
            ValidationFailure f,
            ILpValidationDetailsFormatter detailsFormatter)
        {
            var path = string.Empty;
            if (f.FormattedMessagePlaceholderValues != null &&
                f.FormattedMessagePlaceholderValues.TryGetValue("PropertyPath", out object oPP) &&
                oPP is string sPP)
            {
                path = sPP;
            }
            else if (!string.IsNullOrEmpty(f.PropertyName))
            {
                path = f.PropertyName;
            }

            var code = f.ErrorCode switch
            {
                "NotNullValidator" => ValidationErrorCodes.NotNull,
                "NullValidator" => ValidationErrorCodes.Null,
                "NotEmptyValidator" => ValidationErrorCodes.NotEmpty,
                "EmptyValidator" => ValidationErrorCodes.Empty,
                "NotEqualValidator" => ValidationErrorCodes.NotEqual,
                "EqualValidator" => ValidationErrorCodes.Equal,
                "LengthValidator" => ValidationErrorCodes.Length,
                "ExactLengthValidator" => ValidationErrorCodes.Length,
                "MaximumLengthValidator" => ValidationErrorCodes.MaxLength,
                "MinimumLengthValidator" => ValidationErrorCodes.MinLength,
                "LessThanValidator" => ValidationErrorCodes.LessThan,
                "LessThanOrEqualValidator" => ValidationErrorCodes.LessThanOrEqual,
                "GreaterThanValidator" => ValidationErrorCodes.GreaterThan,
                "GreaterThanOrEqualValidator" => ValidationErrorCodes.GreaterThanOrEqual,
                "RegularExpressionValidator" => ValidationErrorCodes.Regex,
                "EmailValidator" => ValidationErrorCodes.Email,
                "CreditCardValidator" => ValidationErrorCodes.CreditCard,
                "EnumValidator" => ValidationErrorCodes.Enum,
                "StringEnumValidator" => ValidationErrorCodes.Enum,
                "ExclusiveBetweenValidator" => ValidationErrorCodes.ExclusiveBetween,
                "InclusiveBetweenValidator" => ValidationErrorCodes.InclusiveBetween,
                "ScalePrecisionValidator" => ValidationErrorCodes.PrecisionScale,
                "UniquenessValidator" => ValidationErrorCodes.Unique,
                "InternationalPhoneNumberValidator" => ValidationErrorCodes.Phone,
                "InternationalFullNameValidator" => ValidationErrorCodes.FullName,
                "UrlStringValidator" => ValidationErrorCodes.Url,
                "UrlValidator" => ValidationErrorCodes.Url,
                "PrefixValidator" => ValidationErrorCodes.Prefix,
                "SuffixValidator" => ValidationErrorCodes.Suffix,
                "AsyncPredicateValidator" => ValidationErrorCodes.MessagesOnly,
                "PredicateValidator" => ValidationErrorCodes.MessagesOnly,
                _ => f.ErrorCode,
            };
            var message = f.ErrorMessage;
            var details = new Dictionary<string, string>();
            switch (code)
            {
                case ValidationErrorCodes.MessagesOnly:
                    return new LpValidationError(path, message);

                case ValidationErrorCodes.NotNull:
                case ValidationErrorCodes.Null:
                case ValidationErrorCodes.NotEmpty:
                case ValidationErrorCodes.Empty:
                case ValidationErrorCodes.Email:
                case ValidationErrorCodes.CreditCard:
                case ValidationErrorCodes.Enum:
                case ValidationErrorCodes.Phone:
                case ValidationErrorCodes.NotFound:
                case ValidationErrorCodes.AccessDenied:
                case ValidationErrorCodes.Unknown:
                    // No details
                    break;
                case ValidationErrorCodes.NotEqual:
                case ValidationErrorCodes.Equal:
                case ValidationErrorCodes.LessThan:
                case ValidationErrorCodes.LessThanOrEqual:
                case ValidationErrorCodes.GreaterThan:
                case ValidationErrorCodes.GreaterThanOrEqual:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Target,
                            f.FormattedMessagePlaceholderValues["ComparisonValue"]));
                    break;
                case ValidationErrorCodes.Length:
                    details.Add(
                        ValidationErrorCodes.Details.Min,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Min,
                            f.FormattedMessagePlaceholderValues["MinLength"]));
                    details.Add(
                        ValidationErrorCodes.Details.Max,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Max,
                            f.FormattedMessagePlaceholderValues["MaxLength"]));
                    break;
                case ValidationErrorCodes.MaxLength:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Target,
                            f.FormattedMessagePlaceholderValues["MaxLength"]));
                    break;
                case ValidationErrorCodes.MinLength:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Target,
                            f.FormattedMessagePlaceholderValues["MinLength"]));
                    break;
                case ValidationErrorCodes.Regex:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        f.FormattedMessagePlaceholderValues["RegularExpression"].ToString());
                    break;
                case ValidationErrorCodes.ExclusiveBetween:
                case ValidationErrorCodes.InclusiveBetween:
                    details.Add(
                        ValidationErrorCodes.Details.Min,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Min,
                            f.FormattedMessagePlaceholderValues["From"]));
                    details.Add(
                        ValidationErrorCodes.Details.Max,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Max,
                            f.FormattedMessagePlaceholderValues["To"]));
                    break;
                case ValidationErrorCodes.PrecisionScale:
                    details.Add(
                        ValidationErrorCodes.Details.Precision,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Precision,
                            f.FormattedMessagePlaceholderValues["ExpectedPrecision"]));
                    details.Add(
                        ValidationErrorCodes.Details.Scale,
                        detailsFormatter.FormatDetail(
                            path,
                            code,
                            ValidationErrorCodes.Details.Scale,
                            f.FormattedMessagePlaceholderValues["ExpectedScale"]));
                    break;
                case ValidationErrorCodes.Unique:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        f.FormattedMessagePlaceholderValues["MustBeUnique"].ToString());
                    break;
                case ValidationErrorCodes.Url:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        f.FormattedMessagePlaceholderValues["Mode"].ToString());
                    break;
                case ValidationErrorCodes.Prefix:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        f.FormattedMessagePlaceholderValues["Prefix"].ToString());
                    details.Add(
                        ValidationErrorCodes.Details.Target2,
                        f.FormattedMessagePlaceholderValues["MustBePrefixed"].ToString());
                    break;
                case ValidationErrorCodes.Suffix:
                    details.Add(
                        ValidationErrorCodes.Details.Target,
                        f.FormattedMessagePlaceholderValues["Suffix"].ToString());
                    details.Add(
                        ValidationErrorCodes.Details.Target2,
                        f.FormattedMessagePlaceholderValues["MustBeSuffixed"].ToString());
                    break;

                default:
                    if (f.FormattedMessagePlaceholderValues != null)
                    {
                        var customDetails = f.FormattedMessagePlaceholderValues
                            .Where(kvp => kvp.Key.EndsWith(
                                ValidationErrorCodes.Details.LpDetailsPostfix,
                                StringComparison.OrdinalIgnoreCase))
                            .Select(kvp =>
                            {
                                var key = stringService.ToDashedCase(
                                    stringService.SplitCamelCase(
                                    kvp.Key[0..^ValidationErrorCodes.Details.LpDetailsPostfix.Length]));
                                return new
                                {
                                    Key = key,
                                    Value = detailsFormatter.FormatDetail(
                                        path,
                                        code,
                                        key,
                                        kvp.Value)
                                };
                            })
                            .ToList();
                        customDetails.ForEach(d => details.Add(d.Key, d.Value));
                    }
                    break;
            }

            return new LpValidationError(path, code, message, details);
        }
        #endregion
    }
}
