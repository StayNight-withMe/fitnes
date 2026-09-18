using System.Globalization;
using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Profile.EditWeight;

public class UpdateWeightMessageValidator : AbstractValidator<UpdateWeightMessage>
{
    public UpdateWeightMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.Input)
            .NotEmpty()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.InvalidNumber));

        RuleFor(x => x.Input)
            .Must(BeValidWeight)
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.InvalidNumber));
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
