using fitnes.Application.Features.MoveBack;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.MoveBack;

public class CancelHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _botClient;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public CancelHandler(IMediator mediator, ITelegramBotClient botClient, ISessionRepository sessionRepository, IRequestContext context, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _botClient = botClient;
        _sessionRepository = sessionRepository;
        _context = context;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data is string data && data.Equals(CallbackPrefixConstants.Cancel);
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var result = await _mediator.Send(new MoveBackMessage(chatId), cancellationToken);
        await _botClient.SendOrEdit(
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
