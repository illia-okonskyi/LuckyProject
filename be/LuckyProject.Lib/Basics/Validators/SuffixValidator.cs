using FluentValidation.Validators;
using FluentValidation;

namespace LuckyProject.Lib.Basics.Validators
{
    public class SuffixValidator<T> : PropertyValidator<T, string>
    {
        private readonly string suffix;
        private readonly bool mustBeSuffixed;

        public SuffixValidator(string suffix, bool mustBeSuffixed)
        {
            this.suffix = suffix;
            this.mustBeSuffixed = mustBeSuffixed;
        }

        public override string Name => "SuffixValidator";

        public override bool IsValid(ValidationContext<T> context, string value)
        {
            var isSuffixed = value.EndsWith(suffix);
            if (isSuffixed == mustBeSuffixed)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("Suffix", suffix);
            context.MessageFormatter.AppendArgument("MustBeSuffixed", mustBeSuffixed);
            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} suffix {Suffix} is expected = {MustBeSuffixed}";
    }
}
