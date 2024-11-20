using FluentValidation;
using LuckyProject.AuthServer.Services.Roles.Requests;
using LuckyProject.Lib.Basics.Extensions;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Roles.Validators
{
    public class CreateUpdateRoleRequestValidator : AbstractValidator<CreateUpdateRoleRequest>
    {
        public CreateUpdateRoleRequestValidator(HashSet<string> normalizedRoleNames)
        {
            RuleFor(r => r.NormalizedName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128)
                .Unique(normalizedRoleNames);
            RuleFor(r => r.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128);
        }
    }
}
