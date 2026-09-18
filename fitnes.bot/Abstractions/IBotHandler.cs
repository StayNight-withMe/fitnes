using Telegram.Bot.Types;

namespace fitnes.bot.Abstractions;

public interface IBotHandler
{
    bool CanHandle(Update update);
    Task HandleAsync(Update update, CancellationToken ct);
}
