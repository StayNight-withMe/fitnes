using fitnes.bot.Abstractions;
using fitnes.Domain.Constants.Bot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

namespace fitnes.bot;

public class BotWorker : BackgroundService
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<BotWorker> _logger;

    public BotWorker(ITelegramBotClient telegramBotClient, IServiceScopeFactory scopeFactory, ILogger<BotWorker> logger)
    {
        _telegramBotClient = telegramBotClient;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var updateHandler = scope.ServiceProvider.GetRequiredService<IBotUpdateHandler>();
        await updateHandler.HandleUpdateAsync(update, cancellationToken);
    }

    async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, HandleErrorSource source, CancellationToken cancellationToken)
    {
        try
        {
            if (ex is OperationCanceledException && cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug("Telegram polling cancelled ({Source})", source);
                return;
            }

            if (ex is RequestException)
            {
                _logger.LogWarning(ex, "Telegram polling request warning ({Source})", source);
            }
            else
            {
                _logger.LogError(ex, "Telegram polling error ({Source})", source);
            }
        }
        catch (Exception logEx)
        {
            Console.Error.WriteLine($"[HandleErrorAsync fallback] source={source} ex={ex.GetType().Name}: {ex.Message} | logEx={logEx.GetType().Name}: {logEx.Message}");
        }
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telegram polling starting with timeout {TimeoutSeconds}s", BotConstants.TelegramPollingTimeoutSeconds);

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Polling loop starting...");
                await _telegramBotClient.ReceiveAsync(
                    updateHandler: HandleUpdateAsync,
                    errorHandler: (bot, ex, ct) => HandleErrorAsync(bot, ex, HandleErrorSource.PollingError, ct),
                    cancellationToken: cancellationToken
                );

                break;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Polling loop died, restart in {DelaySeconds}s", BotConstants.PollingRestartDelaySeconds);
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(BotConstants.PollingRestartDelaySeconds), cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        _logger.LogInformation("Telegram polling stopped");
    }
}
