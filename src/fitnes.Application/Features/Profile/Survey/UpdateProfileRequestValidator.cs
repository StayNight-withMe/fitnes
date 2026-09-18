using System.Globalization;
using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Profile.Survey;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator(ILocalizer localizer, IRequestContext context)
    {
        var state = context.Session?.State;

        RuleFor(x => x.Input)
            .NotEmpty()
            .WithMessage(ValidatorPhrases.Get(localizer, context, state ?? WorkflowStep.Idle, EmptyMessageKey(state)));

        When(_ => state is WorkflowStep.AwaitingWeight, () =>
        {
            RuleFor(x => x.Input)
                .Must(input => BeInRange(input, ProfileValidationConstants.WeightMin, ProfileValidationConstants.WeightMax))
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingHeight, () =>
        {
            RuleFor(x => x.Input)
                .Must(input => BeInRange(input, ProfileValidationConstants.HeightMin, ProfileValidationConstants.HeightMax))
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingHeight, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingAge, () =>
        {
            RuleFor(x => x.Input)
                .Must(BeValidAge)
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingAge, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingGender, () =>
        {
            RuleFor(x => x.Input)
                .Must(BeValidGender)
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingGender, LocalizationKeysConstants.Profile.InvalidGender));
        });

        When(_ => state is WorkflowStep.AwaitingWaist, () =>
        {
            RuleFor(x => x.Input)
                .Must(input => BeInRange(input, ProfileValidationConstants.MeasurementMin, ProfileValidationConstants.MeasurementMax))
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingWaist, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingNeck, () =>
        {
            RuleFor(x => x.Input)
                .Must(input => BeInRange(input, ProfileValidationConstants.MeasurementMin, ProfileValidationConstants.MeasurementMax))
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingNeck, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingHips, () =>
        {
            RuleFor(x => x.Input)
                .Must(input => BeInRange(input, ProfileValidationConstants.MeasurementMin, ProfileValidationConstants.MeasurementMax))
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingHips, LocalizationKeysConstants.Profile.InvalidNumber));
        });

        When(_ => state is WorkflowStep.AwaitingTimezone, () =>
        {
            RuleFor(x => x.Input)
                .Must(BeValidTimezone)
                .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.AwaitingTimezone, LocalizationKeysConstants.Profile.InvalidTimezone));
        });
    }

    private static string EmptyMessageKey(WorkflowStep? state)
    {
        if (state is WorkflowStep.AwaitingGender)
        {
            return LocalizationKeysConstants.Profile.InvalidGender;
        }

        if (state is WorkflowStep.AwaitingTimezone)
        {
            return LocalizationKeysConstants.Profile.InvalidTimezone;
        }

        return LocalizationKeysConstants.Profile.InvalidNumber;
    }

    private static bool BeInRange(string? input, double min, double max)
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

        return value > min && value <= max;
    }

    private static bool BeValidAge(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        if (!int.TryParse(input.Trim(), out var age))
        {
            return false;
        }

        return age > ProfileValidationConstants.AgeMin && age <= ProfileValidationConstants.AgeMax;
    }

    private static bool BeValidGender(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        var normalized = input.Trim().ToLowerInvariant();

        if (normalized == GenderInputConstants.MaleFull || normalized == GenderInputConstants.MaleShortEn || normalized == GenderInputConstants.MaleShortRu || normalized == GenderInputConstants.MaleRu || normalized == GenderInputConstants.MaleOptionNumber)
        {
            return true;
        }

        if (normalized == GenderInputConstants.FemaleFull || normalized == GenderInputConstants.FemaleShortEn || normalized == GenderInputConstants.FemaleShortRu || normalized == GenderInputConstants.FemaleRu || normalized == GenderInputConstants.FemaleOptionNumber)
        {
            return true;
        }

        return Enum.TryParse<Gender>(normalized, true, out var parsed) && Enum.IsDefined(parsed);
    }

    private static bool BeValidTimezone(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            return true;
        }

        return UpdateProfileTimezoneParser.TryParse(input, out _);
    }
}
