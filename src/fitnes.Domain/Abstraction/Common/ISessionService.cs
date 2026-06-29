using fitnes.Domain.Entities;
using fitnes.Domain.Enums;

namespace fitnes.Domain.Abstraction.Common;

public interface ISessionService
{
    Task<UserSession> UpsertSession(long chatId, WorkflowStep initialState, CancellationToken cancellationToken);
}
