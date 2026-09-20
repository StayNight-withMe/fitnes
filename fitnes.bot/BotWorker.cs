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
        if (ex is RequestException)
        {
            _logger.LogWarning(ex, "Telegram polling request warning ({Source})", source);
        }
        else
        {
            _logger.LogError(ex, "Telegram polling error ({Source})", source);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Telegram polling started with timeout {TimeoutSeconds}s", BotConstants.TelegramPollingTimeoutSeconds);
        _telegramBotClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            cancellationToken: cancellationToken
        );

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
