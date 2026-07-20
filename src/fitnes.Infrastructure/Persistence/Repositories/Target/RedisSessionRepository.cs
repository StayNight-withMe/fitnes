using System.Text.Json;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using StackExchange.Redis;

namespace fitnes.Infrastructure.Persistence.Repositories.Target;

public class RedisSessionRepository : ISessionRepository
{
    private readonly IDatabase _database;
    private const string SessionKeyPrefix = "session:";
    private const string AllSessionsIndexKey = "sessions:index";

    public RedisSessionRepository(IConnectionMultiplexer redis)
    {
        _database = redis.GetDatabase();
    }

    public async Task SaveSession(UserSession session, TimeSpan expiry, CancellationToken cancellationToken)
    {
        var key = GetKey(session.Id);
        var data = JsonSerializer.Serialize(session);

        await _database.StringSetAsync(key, data, expiry);

        await _database.SortedSetAddAsync(
            AllSessionsIndexKey, 
            session.Id, 
            DateTimeOffset.UtcNow.ToUnixTimeSeconds());
    }

    public async Task<UserSession?> GetSession(long chatId, CancellationToken cancellationToken)
    {
        var key = GetKey(chatId);
        var data = await _database.StringGetAsync(key);

        if (data.IsNullOrEmpty)
        {
            return null;
        }

        return JsonSerializer.Deserialize<UserSession>(data!.ToString());
    }

    public async Task DeleteSession(long chatId, CancellationToken cancellationToken)
    {
        var key = GetKey(chatId);

        await _database.KeyDeleteAsync(key);
        await _database.SortedSetRemoveAsync(AllSessionsIndexKey, chatId);
    }

    private string GetKey(long chatId)
    {
        return $"{SessionKeyPrefix}{chatId}";
    }
}
