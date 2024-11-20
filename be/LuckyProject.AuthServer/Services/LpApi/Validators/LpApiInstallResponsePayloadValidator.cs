using FluentValidation;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.Lib.Web.Models.Callback;
using System.Collections.Generic;

namespace LuckyProject.AuthServer.Services.LpApi.Validators
{
    public class LpApiInstallResponsePayloadValidator
        : AbstractValidator<LpApiInstallResponsePayload>
    {
        public LpApiInstallResponsePayloadValidator(HashSet<string> allApiNames)
        {
            RuleFor(m => m.Name)
                .NotNull()
                .NotEmpty()
                .Matches(@"[a-zA-Z_\-]{1,53}")
                .Unique(allApiNames);
            RuleFor(m => m.Description)
                .NotNull()
                .NotEmpty()
                .Length(1, 256);
            RuleFor(m => m.Origins)
                .NotNull()
                .NotEmpty()
                .ForEach(o => o.NotNull().NotEmpty().Url());
            RuleFor(m => m.Endpoint)
                .NotNull()
                .NotEmpty()
                .Url();
        }
    }
}
