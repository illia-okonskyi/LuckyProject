using LuckyProject.Lib.Basics.Constants;
using LuckyProject.Lib.Basics.Extensions;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Services
{
    #region Interface
    public interface ILpValidationDetailsFormatter
    {
        string FormatDetail(string path, string errorCode, string detail, object value);
    }
    #endregion

    #region Impl
    public class LpValidationDetailsFormattter : ILpValidationDetailsFormatter
    {
        private readonly Dictionary<
            (string, string, string),
            (string, IFormatProvider)> customFormats;
        public LpValidationDetailsFormattter(
            Dictionary<(string, string, string), (string, IFormatProvider)> customFormats = null)
        {
            this.customFormats = customFormats ?? new();
        }

        public string FormatDetail(string path, string errorCode, string detail, object value)
        {
            var (format, formatProvider) = GetFormat(path, errorCode, detail);
            return value.ToString(format, formatProvider);
        }

        private (string, IFormatProvider) GetFormat(string path, string errorCode, string detail)
        {
            if (customFormats.TryGetValue((path, errorCode, detail), out var customFormat))
            {
                return customFormat;
            }

            return (errorCode, detail) switch
            {
                (ValidationErrorCodes.NotEqual, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.Equal, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.Length, ValidationErrorCodes.Details.Min) =>
                    ("G", null),
                (ValidationErrorCodes.Length, ValidationErrorCodes.Details.Max) =>
                    ("G", null),
                (ValidationErrorCodes.MinLength, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.MaxLength, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.LessThan, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.LessThanOrEqual, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.GreaterThan, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.GreaterThanOrEqual, ValidationErrorCodes.Details.Target) =>
                    ("G", null),
                (ValidationErrorCodes.ExclusiveBetween, ValidationErrorCodes.Details.Min) =>
                    ("G", null),
                (ValidationErrorCodes.ExclusiveBetween, ValidationErrorCodes.Details.Max) =>
                    ("G", null),
                (ValidationErrorCodes.InclusiveBetween, ValidationErrorCodes.Details.Min) =>
                    ("G", null),
                (ValidationErrorCodes.InclusiveBetween, ValidationErrorCodes.Details.Max) =>
                    ("G", null),
                (ValidationErrorCodes.PrecisionScale, ValidationErrorCodes.Details.Precision) =>
                    ("G", null),
                (ValidationErrorCodes.PrecisionScale, ValidationErrorCodes.Details.Scale) =>
                    ("G", null),

                _ => throw new NotImplementedException()
            };
        }
    }
    #endregion

    #region Builder
    public class LpValidationDetailsFormatterBuilder()
    {
        private readonly Dictionary<
            (string, string, string),
            (string, IFormatProvider)> customFormats = new();

        public LpValidationDetailsFormatterBuilder SetCustomDetailFormat(
            string path,
            string code,
            string detail,
            string format,
            IFormatProvider formatProvider = null)
        {
            customFormats[(path, code, detail)] = (format, formatProvider);
            return this;
        }

        public LpValidationDetailsFormatterBuilder SetNotEqualFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.NotEqual,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetEqualFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.Equal,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetLengthMinFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.Length,
                ValidationErrorCodes.Details.Min,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetLengthMaxFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.Length,
                ValidationErrorCodes.Details.Max,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetMinLengthFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.MinLength,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetMaxLengthFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.MaxLength,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetLessThanFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.LessThan,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetLessThanOrEqualFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.LessThanOrEqual,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetGreaterThanFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.GreaterThan,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetGreaterThanOrEqualFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.GreaterThanOrEqual,
                ValidationErrorCodes.Details.Target,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetExclusiveBetweenMinFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.ExclusiveBetween,
                ValidationErrorCodes.Details.Min,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetExclusiveBetweenMaxFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.ExclusiveBetween,
                ValidationErrorCodes.Details.Max,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetInclusiveBetweenMinFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.InclusiveBetween,
                ValidationErrorCodes.Details.Min,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetInclusiveBetweenMaxFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.InclusiveBetween,
                ValidationErrorCodes.Details.Max,
                format,
                formatProvider);

        public LpValidationDetailsFormatterBuilder SetPrecisionScalePrecisionFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.PrecisionScale,
                ValidationErrorCodes.Details.Precision,
                format,
                formatProvider);


        public LpValidationDetailsFormatterBuilder SetPrecisionScaleScaleFormat(
            string path,
            string format,
            IFormatProvider formatProvider = null) =>
            SetCustomDetailFormat(
                path,
                ValidationErrorCodes.PrecisionScale,
                ValidationErrorCodes.Details.Scale,
                format,
                formatProvider);

        public LpValidationDetailsFormattter Build()
        {
            return new LpValidationDetailsFormattter(customFormats);
        }
    }
    #endregion
}
