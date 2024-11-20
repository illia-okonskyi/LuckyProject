using FluentValidation;
using LuckyProject.AuthServer.Services.OpenId.Requests;
using LuckyProject.Lib.Basics.Extensions;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.OpenId.Validators
{
    public class CreateWebClientRequestValidator : AbstractValidator<CreateWebClientRequest>
    {
        public CreateWebClientRequestValidator(HashSet<string> allClientIds)
        {
            RuleFor(r => r.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(96);
            RuleFor(r => r.DisplayName)
                .NotNull()
                .NotEmpty()
                .MaximumLength(128);
            RuleFor(r => r.ClientId)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100)
                .Unique(allClientIds);
            RuleFor(r => r.Origins)
                .NotNull()
                .NotEmpty()
                .ForEach(o => o.SetValidator(new WebClientOriginValidator()));
        }
    }
}
