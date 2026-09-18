using fitnes.Application.Validation;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using FluentValidation;

namespace fitnes.Application.Features.Language.SetLanguage;

public class SetLanguageMessageValidator : AbstractValidator<SetLanguageMessage>
{
    public SetLanguageMessageValidator(ILocalizer localizer, IRequestContext context)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(ValidatorPhrases.Get(localizer, context, WorkflowStep.LanguageSelection, LocalizationKeysConstants.LanguageSelection.InvalidLanguage));
    }
}
