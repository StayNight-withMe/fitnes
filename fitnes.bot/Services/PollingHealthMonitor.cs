using fitnes.Domain.Constants.Bot;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace fitnes.bot.Services;

public class PollingHealthMonitor : BackgroundService
{
    private readonly PollingHealthState _state;
    private readonly ILogger<PollingHealthMonitor> _logger;

    public PollingHealthMonitor(PollingHealthState state, ILogger<PollingHealthMonitor> logger)
    {
        _state = state;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        int lastSeenRestarts = 0;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(BotConstants.WatchdogIntervalSeconds), cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }

            int restarts = _state.LoopRestarts;
            TimeSpan idleFor = DateTimeOffset.UtcNow - _state.LastUpdateHandledUtc;

            _logger.LogInformation(
                "Polling heartbeat: loop restarts={Restarts}, idle for {IdleMinutes:F1} min",
                restarts, idleFor.TotalMinutes);

            if (idleFor > TimeSpan.FromMinutes(BotConstants.WatchdogIdleWarnMinutes))
            {
                _logger.LogWarning(
                    "No Telegram updates handled for {IdleMinutes:F1} min (may be normal idleness, queue may be stuck if pending_update_count grows)",
                    idleFor.TotalMinutes);
            }

            if (restarts - lastSeenRestarts >= BotConstants.MaxLoopRestartsBeforeExit)
            {
                _logger.LogCritical(
                    "Polling loop died {Deaths} times within {IntervalSeconds}s, exiting for external restart",
                    restarts - lastSeenRestarts, BotConstants.WatchdogIntervalSeconds);
                Environment.Exit(1);
                return;
            }

            lastSeenRestarts = restarts;
        }
    }
}
