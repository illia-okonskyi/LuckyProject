using FluentValidation;
using LuckyProject.AuthServer.Services.OpenId.Constants;
using LuckyProject.AuthServer.Services.OpenId.Requests;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Basics.Validators;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.OpenId.Validators
{
    public class CreateMachineClientRequestValidator : AbstractValidator<CreateMachineClientRequest>
    {
        public CreateMachineClientRequestValidator(HashSet<string> allClientIds)
        {
            RuleFor(r => r.Name)
                .NotNull()
                .NotEmpty()
                .Matches(@"[a-zA-Z0-9\-\._]{1,60}");
            RuleFor(r => r.ClientId)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100)
                .Unique(allClientIds);
            RuleFor(r => r.Origins)
                .NotNull()
                .NotEmpty()
                .ForEach(o => o.Url(UrlValidatorMode.EndpointAddressOnly));
            RuleFor(r => r.Secret)
                .NotNull()
                .NotEmpty()
                .Matches(ServiceConstants.MachineClientSecretRegex.ToString());
        }
    }
}
