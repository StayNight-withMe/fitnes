using Microsoft.Extensions.Options;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Options;
using fitnes.Domain.Enums;
using fitnes.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace fitnes.Application.Services;

public class SessionService : ISessionService
{
    private readonly ISessionRepository _repo;
    private readonly SessionOptions _options;
    private readonly ILogger<SessionService> _logger;

    public SessionService(ISessionRepository repo, IOptions<SessionOptions> options, ILogger<SessionService> logger)
    {
        _repo = repo;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<UserSession> UpsertSession(long chatId, WorkflowStep initialState, CancellationToken cancellationToken)
    {
        var session = await _repo.GetSession(chatId, cancellationToken) 
                      ?? new UserSession { Id = chatId };

        session.State = initialState;
        session.LastUpdate = DateTime.UtcNow;

        var expiry = TimeSpan.FromMinutes(_options.ExpiryMinutes);

        await _repo.SaveSession(session, expiry, cancellationToken);
        await _repo.AddToIndex(chatId, DateTimeOffset.UtcNow, cancellationToken);

        _logger.LogDebug($"Сессия сохранена с значением: {initialState}");

        return session;
    }

    public async Task SaveSession(UserSession session, CancellationToken cancellationToken)
    {
        var expiry = TimeSpan.FromMinutes(_options.ExpiryMinutes);

        await _repo.SaveSession(session, expiry, cancellationToken);
        await _repo.AddToIndex(session.Id, DateTimeOffset.UtcNow, cancellationToken);
    }

    public async Task DeleteSession(long chatId, CancellationToken cancellationToken)
    {
        await _repo.DeleteSession(chatId, cancellationToken);
        await _repo.RemoveFromIndex(chatId, cancellationToken);
    }
}
