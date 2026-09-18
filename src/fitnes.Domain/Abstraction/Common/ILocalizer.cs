using fitnes.Domain.Enums;

namespace fitnes.Domain.Abstraction.Common;

public interface ILocalizer
{
    string GetPhrase(WorkflowStep state, string key);
    string GetPhrase(WorkflowStep state, string key, Language language);
}
