using fitnes.Application.Features.Profile.Survey;
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

public class ProfileHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _botClient;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public ProfileHandler(IMediator mediator, ITelegramBotClient botClient, ISessionRepository sessionRepository, IRequestContext context, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _botClient = botClient;
        _sessionRepository = sessionRepository;
        _context = context;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return _context.Session?.State.IsProfileStep() ?? false;
    }

    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        var text = update.Message!.Text;
        var chatId = update.Message.Chat.Id;
        var result = await _mediator.Send(new UpdateProfileRequest(chatId, text ?? string.Empty), ct);
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
