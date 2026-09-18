using fitnes.Application.Features.Goals.Survey;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Enums;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Goals;

public class GoalsInputHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public GoalsInputHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _sessionRepository = sessionRepository;
        _context = context;
        _botClient = botClient;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.Message?.Text is string && _context.Session?.State is WorkflowStep.AwaitingGoalWeight;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.Message!.Chat.Id;
        var result = await _mediator.Send(new UpdateGoalMessage(chatId, update.Message!.Text!), cancellationToken);
        await _botClient.SendAndDelete(
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
