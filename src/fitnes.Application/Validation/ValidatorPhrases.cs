using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;

namespace fitnes.Application.Validation;

public static class ValidatorPhrases
{
    public static string Get(ILocalizer localizer, IRequestContext context, WorkflowStep step, string key)
    {
        if (context.Session?.Language is Language language)
        {
            return localizer.GetPhrase(step, key, language);
        }

        return localizer.GetPhrase(step, key, LocalizationConstants.DefaultLanguage);
    }
}
