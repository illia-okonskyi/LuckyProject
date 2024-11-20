using FluentValidation.Validators;
using FluentValidation;
using System;

namespace LuckyProject.Lib.Basics.Validators
{
    public enum UrlValidatorMode
    {
        Default,
        EndpointAddressOnly,
        RelativeOnly,
        AbsoluteOnly
    }

    public class UrlStringValidator<T> : PropertyValidator<T, string>
    {
        private readonly UrlValidatorMode mode;

        public UrlStringValidator(UrlValidatorMode mode)
        {
            this.mode = mode;
        }

        public override string Name => "UrlStringValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            var result = IsValid(value);
            if (!result)
            {
                context.MessageFormatter.AppendArgument("Mode", mode.ToString());
            }

            return result;
        }

        private bool IsValid(string value)
        {
            var kind = mode switch
            {
                UrlValidatorMode.Default => UriKind.RelativeOrAbsolute,
                UrlValidatorMode.EndpointAddressOnly => UriKind.Absolute,
                UrlValidatorMode.RelativeOnly => UriKind.Relative,
                UrlValidatorMode.AbsoluteOnly => UriKind.Absolute,
                _ => throw new InvalidOperationException("Invalid mode")
            };

            if (!Uri.TryCreate(value, kind, out var url))
            {
                return false;
            }

            return mode == UrlValidatorMode.EndpointAddressOnly
                ? url.PathAndQuery == "/"
                : true;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} must be URL in {Mode}";
    }

    public class UrlValidator<T> : PropertyValidator<T, Uri>
    {
        private readonly int? maxLength;
        private readonly UrlValidatorMode mode;

        public UrlValidator(UrlValidatorMode mode, int? maxLength)
        {
            this.mode = mode;
            this.maxLength = maxLength;
        }

        public override string Name => "UrlValidator";

        public override bool IsValid(ValidationContext<T> context, Uri value)
        {
            var result = IsValid(value);
            if (!result)
            {
                context.MessageFormatter.AppendArgument("Mode", mode.ToString());
            }

            return result;
        }

        private bool IsValid(Uri value)
        {
            if (maxLength.HasValue && value.ToString().Length > maxLength.Value)
            {
                return false;
            }

            bool? expectedIsAbsolute = mode switch
            {
                UrlValidatorMode.Default => null,
                UrlValidatorMode.EndpointAddressOnly => true,
                UrlValidatorMode.RelativeOnly => false,
                UrlValidatorMode.AbsoluteOnly => true,
                _ => throw new InvalidOperationException("Invalid mode")
            };

            if (!expectedIsAbsolute.HasValue)
            {
                return true;
            }

            if (value.IsAbsoluteUri != expectedIsAbsolute)
            {
                return false;
            }

            return mode == UrlValidatorMode.EndpointAddressOnly
                ? value.PathAndQuery == "/"
                : true;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} must be URL in {Mode}";
    }
}
