using FluentValidation.Validators;
using FluentValidation;
using System.Text.RegularExpressions;

namespace LuckyProject.Lib.Basics.Validators
{
    public class InternationalFullNameValidator<T> : PropertyValidator<T, string>
    {
        private static readonly Regex regex = new(
            @"^[a-zA-Z0-9\ \,\.\'\-]{1,128}$",
            RegexOptions.Compiled);

        public override string Name => "InternationalFullNameValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            return regex.IsMatch(value);
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} must be international full name up to 128 chars";
    }
}
