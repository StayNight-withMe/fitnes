namespace fitnes.Domain.Constants.Bot;

public static class BotConstants
{
    public const string StartCommand = "/start";
    public const int TelegramPollingTimeoutSeconds = 60;
    public const int PollingRestartDelaySeconds = 5;
    public const int WatchdogIntervalSeconds = 60;
    public const int WatchdogIdleWarnMinutes = 10;
    public const int MaxLoopRestartsBeforeExit = 5;
}