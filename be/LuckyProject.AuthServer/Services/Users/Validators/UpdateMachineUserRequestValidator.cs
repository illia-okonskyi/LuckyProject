using FluentValidation;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.Lib.Basics.Extensions;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Users.Validators
{
    public class UpdateMachineUserRequestValidator : AbstractValidator<UpdateMachineUserRequest>
    {
        public UpdateMachineUserRequestValidator(HashSet<string> normalizedEmails)
        {
            RuleFor(r => r.NormalizedEmail)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128)
                .Unique(normalizedEmails);
            RuleFor(r => r.PhoneNumber)
                .NotNull()
                .NotEmpty()
                .InternationalPhoneNumber();
            RuleFor(r => r.PreferredLocale)
                .NotNull()
                .NotEmpty()
                .Length(2, 8);
        }
    }
}
