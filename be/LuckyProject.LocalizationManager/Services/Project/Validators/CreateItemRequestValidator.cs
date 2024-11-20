using FluentValidation;
using LuckyProject.Lib.Basics.Extensions;
using LuckyProject.LocalizationManager.Services.Project.Requests;
using System.Collections.Generic;

namespace LuckyProject.LocalizationManager.Services.Project.Validators
{
    public class CreateItemRequestValidator : AbstractValidator<CreateItemRequest>
    {
        public CreateItemRequestValidator(HashSet<string> allKeys)
        {
            RuleFor(r => r.ResourceType)
                .IsInEnum();
            RuleFor(r => r.Key)
                .NotNull()
                .NotEmpty()
                .Length(1, 512)
                .Matches(@"^[a-zA-Z0-9\-_\.]{1,512}$")
                .Unique(allKeys);
        }
    }
}
