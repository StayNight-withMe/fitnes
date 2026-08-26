using fitnes.Application.Features.Start;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Start;

public class StartCommandHandler : IBotHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public StartCommandHandler(ITelegramBotClient telegramBotClient, IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _telegramBotClient = telegramBotClient;
        _mediator = mediator;
        _sessionRepository = sessionRepository;
        _context = context;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.Message?.Text is Domain.Constants.Bot.BotConstants.StartCommand;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Chat.Id: long chatId })
        {
            var result = await _mediator.Send(new StartMessage(chatId), cancellationToken);
            await _telegramBotClient.SendOrEdit(
                chatId: chatId,
                text: result.GetTextOrErrors(),
                replyMarkup: result.ToTelegramMarkup(),
                sessionRepository: _sessionRepository,
                context: _context,
                sessionOptions: _sessionOptions,
                cancellationToken: cancellationToken
            );
        }
    }
}
