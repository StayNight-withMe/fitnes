using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Goals.SetActivity;

public class GoalActivityMessageValidator : AbstractValidator<GoalActivityMessage>
{
    public GoalActivityMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.Activity)
            .IsInEnum()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.Goals, LocalizationKeysConstants.Goals.InvalidRequest));

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.Goals, LocalizationKeysConstants.Goals.InvalidRequest));

        RuleFor(x => x.TargetWeight)
            .GreaterThan(ProfileValidationConstants.WeightMin)
            .LessThanOrEqualTo(ProfileValidationConstants.WeightMax)
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Profile.InvalidNumber));
    }
}
