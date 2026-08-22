using fitnes.Application.Features.Language.SetLanguage;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Enums;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Languages;

public class LanguageSetHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<LanguageSetHandler> _logger;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public LanguageSetHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, ILogger<LanguageSetHandler> logger, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _mediator = mediator;
        _sessionRepository = sessionRepository;
        _context = context;
        _botClient = botClient;
        _logger = logger;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.CallbackQuery?.Data is string data && data.StartsWith($"{CallbackPrefixConstants.LanguageSelection}{CallbackPrefixConstants.Separator}");
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var data = update.CallbackQuery!.Data!;
        var value = data.Split(CallbackPrefixConstants.Separator)[1];
        _logger.LogDebug($"data from button: {data}");
        var language = Enum.Parse<Language>(value, ignoreCase: true);
        var result = await _mediator.Send(new SetLanguageMessage(chatId, language), cancellationToken);
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
