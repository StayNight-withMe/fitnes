using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Meals.MealStats;

public class MealStatsMessageValidator : AbstractValidator<MealStatsMessage>
{
    public MealStatsMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.Granularity)
            .IsInEnum()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.Stats, LocalizationKeysConstants.Stats.InvalidPeriod));

        RuleFor(x => x.Offset)
            .LessThanOrEqualTo(ValidatorConstants.MaxStatsOffset)
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.Stats, LocalizationKeysConstants.Stats.InvalidPeriod));
    }
}
