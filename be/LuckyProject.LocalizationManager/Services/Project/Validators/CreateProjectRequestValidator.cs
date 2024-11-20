using FluentValidation;
using LuckyProject.LocalizationManager.Services.Project.Requests;

namespace LuckyProject.LocalizationManager.Services.Project.Validators
{
    public class CreateProjectRequestValidator : AbstractValidator<CreateProjectRequest>
    {
        public CreateProjectRequestValidator()
        {
            RuleFor(r => r.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(64);
            RuleFor(r => r.Description)
                .NotNull()
                .NotEmpty()
                .MaximumLength(512);
            RuleFor(r => r.Source)
                .NotNull()
                .NotEmpty()
                .MaximumLength(64);
            RuleFor(r => r.Locale)
                .NotNull()
                .NotEmpty()
                .MaximumLength(32);
            RuleFor(r => r.FilePath)
                .NotNull()
                .NotEmpty();
        }
    }
}
