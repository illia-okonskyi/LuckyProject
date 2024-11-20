using FluentValidation.Validators;
using FluentValidation;

namespace LuckyProject.Lib.Basics.Validators
{
    public class PrefixValidator<T> : PropertyValidator<T, string>
    {
        private readonly string prefix;
        private readonly bool mustBePrefixed;

        public PrefixValidator(string prefix, bool mustBePrefixed)
        {
            this.prefix = prefix;
            this.mustBePrefixed = mustBePrefixed;
        }

        public override string Name => "PrefixValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            var isPrefixed = value.StartsWith(prefix);
            if (isPrefixed == mustBePrefixed)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("Prefix", prefix);
            context.MessageFormatter.AppendArgument("MustBePrefixed", mustBePrefixed);
            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} prefix {Prefix} is expected = {MustBePrefixed}";
    }
}
