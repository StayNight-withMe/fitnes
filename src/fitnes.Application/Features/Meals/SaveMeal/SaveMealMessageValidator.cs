using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Meals.SaveMeal;

public class SaveMealMessageValidator : AbstractValidator<SaveMealMessage>
{
    public SaveMealMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.AnalysisResultId)
            .NotEmpty()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AnalysisResult, LocalizationKeysConstants.AnalysisResult.NotFound));
    }
}
