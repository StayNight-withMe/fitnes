using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Enums;

namespace fitnes.Domain.Entities;

public class UserSession : IEntity<long>
{
    /// <summary>
    /// The unique identifier for the session, corresponding to the user's Telegram ChatId.
    /// </summary>
    public required long Id { get; set; }
    public WorkflowStep State { get; set; }
    public Language Language { get; set; }
    public DateTime LastUpdate { get; set; }
    public int? LastMessageId { get; set; }
}
