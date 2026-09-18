using Telegram.Bot.Types;

namespace fitnes.bot.Abstractions;

public interface IBotUpdateHandler
{
    Task HandleUpdateAsync(Update update, CancellationToken cancellationToken);
}