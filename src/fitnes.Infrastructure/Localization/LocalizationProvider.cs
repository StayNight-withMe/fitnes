using System.Collections.Frozen;
using System.Text.Json;
using fitnes.Domain.Enums;
using fitnes.Domain.Constants.Localization;

namespace fitnes.Infrastructure.Localization;

public class LocalizationProvider
{
    public FrozenDictionary<Language, FrozenDictionary<WorkflowStep, FrozenDictionary<string, string>>> Dictionary { get; }

    public LocalizationProvider()
    {
        var mainBuilder = new Dictionary<Language, FrozenDictionary<WorkflowStep, FrozenDictionary<string, string>>>();

        foreach (var language in Enum.GetValues<Language>())
        {
            var languageCode = language.ToString().ToLowerInvariant();
            var filePath = Path.Combine(AppContext.BaseDirectory, LocalizationConstants.LocalizationPath, LocalizationConstants.ResourcesPath, $"{languageCode}.json");

            if (File.Exists(filePath))
            {
                var jsonContent = File.ReadAllText(filePath);
                var dict = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, string>>>(jsonContent);

                if (dict is not null)
                {
                    var stateBuilder = new Dictionary<WorkflowStep, FrozenDictionary<string, string>>();

                    foreach (var stateEntry in dict)
                    {
                        if (Enum.TryParse<WorkflowStep>(stateEntry.Key, true, out var state))
                        {
                            var phrasesBuilder = new Dictionary<string, string>();

                            foreach (var phrase in stateEntry.Value)
                            {
                                phrasesBuilder.TryAdd(phrase.Key, phrase.Value);
                            }

                            stateBuilder.TryAdd(state, phrasesBuilder.ToFrozenDictionary());
                        }
                    }

                    mainBuilder.TryAdd(language, stateBuilder.ToFrozenDictionary());
                }
            }
        }

        Dictionary = mainBuilder.ToFrozenDictionary();
    }
}
