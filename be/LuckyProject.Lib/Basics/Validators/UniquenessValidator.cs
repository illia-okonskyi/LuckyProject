using FluentValidation.Validators;
using FluentValidation;
using System.Collections.Generic;

namespace LuckyProject.Lib.Basics.Validators
{
    public class UniquenessValidator<T, TProperty> : PropertyValidator<T, TProperty>
    {
        private readonly HashSet<TProperty> values;
        private readonly bool mustBeUnique;

        public UniquenessValidator(HashSet<TProperty> values, bool mustBeUnique = true)
        {
            this.values = values;
            this.mustBeUnique = mustBeUnique;
        }

        public override string Name => "UniquenessValidator";

        public override bool IsValid(ValidationContext<T> context, TProperty value)
        {
            var isUnique = !values.Contains(value);
            if (isUnique == mustBeUnique)
            {
                return true;
            }

            context.MessageFormatter.AppendArgument("MustBeUnique", mustBeUnique);
            return false;
        }

        protected override string GetDefaultMessageTemplate(string errorCode) =>
            "{PropertyName} must uniqueness must be {MustBeUnique}";
    }
}
