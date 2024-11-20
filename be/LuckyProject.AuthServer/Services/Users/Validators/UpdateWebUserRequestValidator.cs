using FluentValidation;
using LuckyProject.AuthServer.Services.Users.Requests;
using System.Collections.Generic;
using LuckyProject.Lib.Basics.Extensions;

namespace LuckyProject.AuthServer.Services.Users.Validators
{
    public class UpdateWebUserRequestValidator : AbstractValidator<UpdateWebUserRequest>
    {
        public UpdateWebUserRequestValidator(
            HashSet<string> normalizedEmails,
            HashSet<string> tgUserNames)
        {
            RuleFor(r => r.NormalizedEmail)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128)
                .EmailAddress()
                .Unique(normalizedEmails);
            RuleFor(r => r.PhoneNumber)
                .NotNull()
                .NotEmpty()
                .InternationalPhoneNumber();
            RuleFor(r => r.FullName)
                .NotNull()
                .NotEmpty()
                .InternationalFullName();
            RuleFor(r => r.TelegramUserName)
                .NotNull()
                .NotEmpty()
                .Length(2, 128)
                .Prefixed("@")
                .Unique(tgUserNames);
            RuleFor(r => r.PreferredLocale)
                .NotNull()
                .NotEmpty()
                .Length(2, 8);
        }
    }
}
