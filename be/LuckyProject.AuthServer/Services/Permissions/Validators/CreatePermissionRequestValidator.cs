using FluentValidation;
using LuckyProject.AuthServer.Services.Permissions.Requests;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Models;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.Permissions.Validators
{
    public class CreatePermissionRequestValidator : AbstractValidator<CreatePermissionRequest>
    {
        public CreatePermissionRequestValidator(HashSet<string> permissionFullNames)
        {
            RuleFor(r => r.Type)
                .IsInEnum();
            RuleFor(r => r.FullName)
                .NotNull()
                .NotEmpty()
                .Matches(@"^[a-zA-Z0-9\-_]{2,64}$")
                .Unique(permissionFullNames);
            RuleFor(r => r.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128);
            RuleFor(r => r.Allow)
                .NotNull()
                .When(r => r.Type == LpAuthPermissionType.Binary);
            RuleFor(r => r.Level)
                .NotNull()
                .NotEmpty()
                .GreaterThanOrEqualTo(2)
                .When(r => r.Type == LpAuthPermissionType.Level);
            RuleFor(r => r.Passkeys)
                .NotNull()
                .NotEmpty()
                .ForEach(pk => pk
                    .NotNull()
                    .NotEmpty()
                    .Matches(@"^[a-zA-Z0-9\`\~\!\@\#\$\%\^\&\*\(\)\-_\=\+\[\]\'\""\<\>\/\?]{16,64}$"))
                .When(r => r.Type == LpAuthPermissionType.Passkey);
        }
    }
}
