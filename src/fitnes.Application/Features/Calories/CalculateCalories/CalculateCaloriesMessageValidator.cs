using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Calories.CalculateCalories;

public class CalculateCaloriesMessageValidator : AbstractValidator<CalculateCaloriesMessage>
{
    public CalculateCaloriesMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.ImageBytes)
            .NotEmpty()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.NoAnalysis));
    }
}
