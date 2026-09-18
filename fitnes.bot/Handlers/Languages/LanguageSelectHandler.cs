using fitnes.Application.Features.Language.SelectLanguage;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Languages;

public class LanguageSelectHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ITelegramBotClient _botClient;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public LanguageSelectHandler(IMediator mediator, ITelegramBotClient botClient, ISessionRepository sessionRepository, IRequestContext context, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _botClient = botClient;
        _sessionRepository = sessionRepository;
        _context = context;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data is string data && data.Equals(Domain.Constants.Bot.CallbackPrefixConstants.Language);
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var result = await _mediator.Send(new SelectLanguageMessage(chatId), cancellationToken);
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
