using FluentValidation;
using LuckyProject.Lib.Basics.Validators;
using System;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Extensions
{
    public static class FluentValidationExtensions
    {
        public static IRuleBuilderOptions<T, TProperty> Unique<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            HashSet<TProperty> values) =>
                ruleBuilder.SetValidator(new UniquenessValidator<T, TProperty>(values, true));

        public static IRuleBuilderOptions<T, TProperty> NotUnique<T, TProperty>(
            this IRuleBuilder<T, TProperty> ruleBuilder,
            HashSet<TProperty> values) =>
                ruleBuilder.SetValidator(new UniquenessValidator<T, TProperty>(values, false));

        public static IRuleBuilderOptions<T, string> InternationalPhoneNumber<T>(
            this IRuleBuilder<T, string> ruleBuilder) =>
                ruleBuilder.SetValidator(new InternationalPhoneNumberValidator<T>());

        public static IRuleBuilderOptions<T, string> InternationalFullName<T>(
            this IRuleBuilder<T, string> ruleBuilder) =>
                ruleBuilder.SetValidator(new InternationalFullNameValidator<T>());

        public static IRuleBuilderOptions<T, string> Url<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            UrlValidatorMode mode = UrlValidatorMode.Default) =>
                ruleBuilder.SetValidator(new UrlStringValidator<T>(mode));
        public static IRuleBuilderOptions<T, Uri> Url<T>(
            this IRuleBuilder<T, Uri> ruleBuilder,
            UrlValidatorMode mode = UrlValidatorMode.Default,
            int? maxLength = null) =>
                ruleBuilder.SetValidator(new UrlValidator<T>(mode, maxLength));

        public static IRuleBuilderOptions<T, string> Prefixed<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string prefix) =>
                ruleBuilder.SetValidator(new PrefixValidator<T>(prefix, true));
        public static IRuleBuilderOptions<T, string> NotPrefixed<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string prefix) =>
                ruleBuilder.SetValidator(new PrefixValidator<T>(prefix, false));
        public static IRuleBuilderOptions<T, string> Suffixed<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string suffix) =>
                ruleBuilder.SetValidator(new SuffixValidator<T>(suffix, true));
        public static IRuleBuilderOptions<T, string> NotSuffixed<T>(
            this IRuleBuilder<T, string> ruleBuilder,
            string suffix) =>
                ruleBuilder.SetValidator(new SuffixValidator<T>(suffix, false));
    }
}
