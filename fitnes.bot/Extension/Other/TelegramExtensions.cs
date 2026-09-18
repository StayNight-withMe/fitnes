using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Options;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;

namespace fitnes.bot.Extension.Other;

public static class TelegramExtensions
{
    public static async Task SendOrEdit(
        this ITelegramBotClient botClient,
        long chatId,
        string text,
        InlineKeyboardMarkup? replyMarkup,
        ISessionRepository sessionRepository,
        IRequestContext context,
        IOptionsMonitor<SessionOptions> sessionOptions,
        CancellationToken cancellationToken)
    {
        var session = context.Session;
        if (session is { LastMessageId: int lastMessageId })
        {
            try
            {
                await botClient.EditMessageText(chatId, lastMessageId, text, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
                return;
            }
            catch
            {
                try
                {
                    await botClient.DeleteMessage(chatId, lastMessageId, cancellationToken);
                }
                catch
                {
                }
            }
        }

        var message = await botClient.SendMessage(chatId, text, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        if (session is not null)
        {
            session.LastMessageId = message.Id;
            var expiry = TimeSpan.FromMinutes(sessionOptions.CurrentValue.ExpiryMinutes);
            await sessionRepository.SaveSession(session, expiry, cancellationToken);
        }
    }

    public static async Task SendAndDelete(
        this ITelegramBotClient botClient,
        long chatId,
        string text,
        InlineKeyboardMarkup? replyMarkup,
        ISessionRepository sessionRepository,
        IRequestContext context,
        IOptionsMonitor<SessionOptions> sessionOptions,
        CancellationToken cancellationToken)
    {
        var session = context.Session;
        if (session is { LastMessageId: int lastMessageId })
        {
            try
            {
                await botClient.DeleteMessage(chatId, lastMessageId, cancellationToken);
            }
            catch { }
        }

        var message = await botClient.SendMessage(chatId, text, replyMarkup: replyMarkup, cancellationToken: cancellationToken);
        if (session is not null)
        {
            session.LastMessageId = message.Id;
            var expiry = TimeSpan.FromMinutes(sessionOptions.CurrentValue.ExpiryMinutes);
            await sessionRepository.SaveSession(session, expiry, cancellationToken);
        }
    }
}
