using FluentValidation;
using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Services.Users.Constants;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace LuckyProject.AuthServer.Services.Users.Validators
{
    public class CreateWebUserRequestValidator : AbstractValidator<CreateWebUserRequest>
    {
        private readonly UserManager<AuthServerUser> userManager;

        public CreateWebUserRequestValidator(
            UserManager<AuthServerUser> userManager,
            HashSet<string> normalizedUserNames,
            HashSet<string> normalizedEmails,
            HashSet<string> tgUserNames)
        {
            this.userManager = userManager;

            RuleFor(r => r.UserName)
                .NotNull()
                .NotEmpty()
                .NotPrefixed(ServiceConstants.MachineUserPrefix);
            RuleFor(r => r.User).MustAsync(ValidateUserAsync);
            RuleFor(r => r.NormalizedUserName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(64)
                .Unique(normalizedUserNames);
            RuleFor(r => r.NormalizedEmail)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128)
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
            RuleFor(r => r.Password).MustAsync(ValidatePasswordAsync);
        }

        private async Task<bool> ValidateUserAsync(
            CreateWebUserRequest request,
            AuthServerUser u,
            ValidationContext<CreateWebUserRequest> ctx,
            CancellationToken ct)
        {
            foreach (var v in userManager.UserValidators)
            {
                var ir = await v.ValidateAsync(userManager, u);
                if (ir.UpdateValidationContext("NormalizedUserName", "NormalizedUserName", ctx))
                {
                    return false;
                }
            }

            return true;
        }

        private async Task<bool> ValidatePasswordAsync(
            CreateWebUserRequest request,
            string pwd,
            ValidationContext<CreateWebUserRequest> ctx,
            CancellationToken ct)
        {
            foreach (var v in userManager.PasswordValidators)
            {
                var ir = await v.ValidateAsync(userManager, request.User, pwd);
                if (ir.UpdateValidationContext("Password", "Password", ctx))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
