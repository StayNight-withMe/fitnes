using fitnes.Application.Features.Profile.StartSurvey;
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

namespace fitnes.bot.Handlers.Profile;

public class ProfileEditHandler : IBotHandler
{
    private readonly ITelegramBotClient _botClient;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public ProfileEditHandler(ITelegramBotClient botClient, ISessionRepository sessionRepository, IRequestContext context, IMediator mediator, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _botClient = botClient;
        _sessionRepository = sessionRepository;
        _context = context;
        _mediator = mediator;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data is CallbackPrefixConstants.ProfileEdit;
    }

    public async Task HandleAsync(Update update, CancellationToken ct)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var result = await _mediator.Send(new StartSurveyPromptRequest(chatId), ct);
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
