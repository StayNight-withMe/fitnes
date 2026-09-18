using fitnes.bot.Abstractions;
using Microsoft.Extensions.Logging;
using Telegram.Bot.Types;

namespace fitnes.bot;

public class  BotUpdateHandler : IBotUpdateHandler
{
    private readonly IEnumerable<IBotHandler> _handlers;
    private readonly ILogger<BotUpdateHandler> _logger;

    public BotUpdateHandler(IEnumerable<IBotHandler> handlers, ILogger<BotUpdateHandler> logger)
    {
        _handlers = handlers;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        var handler = _handlers.FirstOrDefault(h => h.CanHandle(update));

        if (handler is not null)
        {
            await handler.HandleAsync(update, cancellationToken);
        }
        else
        {
            _logger.LogDebug("Handler not found for this update.");
        }
    }
}
