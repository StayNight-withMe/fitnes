using System.Globalization;
using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Goals.Survey;

public class UpdateGoalMessageValidator : AbstractValidator<UpdateGoalMessage>
{
    public UpdateGoalMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.Input)
            .NotEmpty()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Profile.InvalidNumber));

        RuleFor(x => x.Input)
            .Must(BeValidWeight)
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Profile.InvalidNumber));
    }

    private static bool BeValidWeight(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        var normalized = input.Trim().Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot);

        if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out var value))
        {
            return false;
        }

        return value > ProfileValidationConstants.WeightMin && value <= ProfileValidationConstants.WeightMax;
    }
}
