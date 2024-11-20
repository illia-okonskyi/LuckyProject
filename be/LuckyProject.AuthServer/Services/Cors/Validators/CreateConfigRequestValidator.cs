using FluentValidation;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.AuthServer.Services.Cors.Requests;
using LuckyProject.Lib.Basics.Validators;

namespace LuckyProject.AuthServer.Services.Cors.Validators
{
    public class CreateConfigRequestValidator : AbstractValidator<CreateConfigRequest>
    {
        public CreateConfigRequestValidator()
        {
            RuleFor(r => r.ClientId)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);
            RuleFor(r => r.Origins)
                .NotNull()
                .NotEmpty()
                .ForEach(o => o
                    .NotNull()
                    .NotEmpty()
                    .Url(UrlValidatorMode.EndpointAddressOnly, 128));
        }
    }
}
