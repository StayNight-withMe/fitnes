using fitnes.Application.Features.Goals.Menu;
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

namespace fitnes.bot.Handlers.Goals;

public class GoalsMenuHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public GoalsMenuHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _sessionRepository = sessionRepository;
        _context = context;
        _botClient = botClient;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data is string data && data.Equals(CallbackPrefixConstants.Goals);
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var result = await _mediator.Send(new GoalsMenuMessage(chatId), cancellationToken);
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
