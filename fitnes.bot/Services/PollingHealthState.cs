namespace fitnes.bot.Services;

public class PollingHealthState
{
    private readonly object _lock = new();
    private DateTimeOffset _lastUpdateHandledUtc = DateTimeOffset.UtcNow;
    private DateTimeOffset _loopStartedUtc = DateTimeOffset.UtcNow;
    private int _loopRestarts;
    private int? _lastUpdateId;

    public DateTimeOffset LastUpdateHandledUtc
    {
        get { lock (_lock) { return _lastUpdateHandledUtc; } }
    }

    public DateTimeOffset LoopStartedUtc
    {
        get { lock (_lock) { return _loopStartedUtc; } }
    }

    public int LoopRestarts
    {
        get { lock (_lock) { return _loopRestarts; } }
    }

    public void MarkUpdateHandled()
    {
        lock (_lock) { _lastUpdateHandledUtc = DateTimeOffset.UtcNow; }
    }

    public void MarkUpdateHandled(int updateId)
    {
        lock (_lock)
        {
            _lastUpdateHandledUtc = DateTimeOffset.UtcNow;
            _lastUpdateId = updateId;
        }
    }

    public int? GetResumeOffset()
    {
        lock (_lock) { return _lastUpdateId.HasValue ? _lastUpdateId.Value + 1 : null; }
    }

    public void MarkLoopStarted()
    {
        lock (_lock) { _loopStartedUtc = DateTimeOffset.UtcNow; }
    }

    public void MarkLoopRestart()
    {
        lock (_lock) { _loopRestarts++; }
    }
}
