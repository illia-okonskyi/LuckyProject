using FluentValidation;
using LuckyProject.AuthServer.DbLayer;
using LuckyProject.AuthServer.Services.Users.Requests;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Web.Extensions;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace LuckyProject.AuthServer.Services.Users.Validators
{
    public class CreateMachineUserRequestValidator : AbstractValidator<CreateMachineUserRequest>
    {
        private readonly UserManager<AuthServerUser> userManager;

        public CreateMachineUserRequestValidator(
            UserManager<AuthServerUser> userManager,
            HashSet<string> normalizedUserNames,
            HashSet<string> normalizedEmails)
        {
            this.userManager = userManager;

            RuleFor(r => r.ClientName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(60);
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
            RuleFor(r => r.PreferredLocale)
                .NotNull()
                .NotEmpty()
                .Length(2, 8);
        }

        private async Task<bool> ValidateUserAsync(
            CreateMachineUserRequest request,
            AuthServerUser u,
            ValidationContext<CreateMachineUserRequest> ctx,
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
    }
}
