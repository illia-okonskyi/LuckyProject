using FluentValidation.Validators;
using FluentValidation;
using System.Text.RegularExpressions;

namespace LuckyProject.Lib.Basics.Validators
{
    public class InternationalPhoneNumberValidator<T> : PropertyValidator<T, string>
    {
        private static readonly Regex regex = new(@"^\+\d{2,15}$", RegexOptions.Compiled);

        public override string Name => "InternationalPhoneNumberValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            return regex.IsMatch(value);
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} must be international phone number";
    }
}
