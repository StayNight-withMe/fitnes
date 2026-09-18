using fitnes.Application.Features.Profile.EditWeight;
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

namespace fitnes.bot.Handlers.Profile;

public class WeightInputHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _botClient;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public WeightInputHandler(IMediator mediator, ITelegramBotClient botClient, ISessionRepository sessionRepository, IRequestContext context, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _botClient = botClient;
        _sessionRepository = sessionRepository;
        _context = context;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.Message?.Text is not null && _context.Session?.State is WorkflowStep.AwaitingWeightEdit;
    }

    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        var chatId = update.Message!.Chat.Id;
        var result = await _mediator.Send(new UpdateWeightMessage(chatId, update.Message!.Text!), ct);
        await _botClient.SendAndDelete(
            chatId: chatId,
            text: result.GetTextOrErrors(),
            replyMarkup: result.ToTelegramMarkup(),
            sessionRepository: _sessionRepository,
            context: _context,
            sessionOptions: _sessionOptions,
            cancellationToken: ct
        );
    }
}
