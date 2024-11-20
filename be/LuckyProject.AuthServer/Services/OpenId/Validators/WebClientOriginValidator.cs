using FluentValidation;
using LuckyProject.AuthServer.Services.OpenId.Models;
using LuckyProject.Lib.Basics.Validators;
using LuckyProject.Lib.Basics.Extensions;

namespace LuckyProject.AuthServer.Services.OpenId.Validators
{
    public class WebClientOriginValidator : AbstractValidator<WebClientOrigin>
    {
        public WebClientOriginValidator()
        {
            RuleFor(o => o.BaseUrl)
                .NotNull()
                .NotEmpty()
                .Url(UrlValidatorMode.EndpointAddressOnly, 128);
        }
    }
}
