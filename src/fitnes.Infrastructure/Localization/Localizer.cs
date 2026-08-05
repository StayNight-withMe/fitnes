using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Enums;

namespace fitnes.Infrastructure.Localization;

public class Localizer : ILocalizer
{
    private readonly LocalizationProvider _provider;
    private readonly IRequestContext _context;

    public Localizer(LocalizationProvider provider, IRequestContext context)
    {
        _provider = provider;
        _context = context;
    }

    public string GetPhrase(WorkflowStep state, string key)
    {
        var language = _context.Session.Language;
        return GetPhrase(state, key, language);
    }

    public string GetPhrase(WorkflowStep state, string key, Language language)
    {
        if (_provider.Dictionary.TryGetValue(language, out var states) &&
            states.TryGetValue(state, out var phrases) &&
            phrases.TryGetValue(key, out var phrase))
        {
            return phrase;
        }

        return $"[{state}:{key}]";
    }
}
