using fitnes.bot.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
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

    async Task HandleErrorAsync(ITelegramBotClient bot, Exception ex, CancellationToken cancellationToken)
    {
        _logger.LogDebug($"Error: {ex.Message}");
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        _telegramBotClient.StartReceiving(
            updateHandler: HandleUpdateAsync,
            errorHandler: HandleErrorAsync,
            cancellationToken: cancellationToken
        );

        await Task.Delay(Timeout.Infinite, cancellationToken);
    }
}
