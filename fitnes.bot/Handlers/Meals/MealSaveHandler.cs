using fitnes.Application.Features.Meals.SaveMeal;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Meals;

public class MealSaveHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<MealSaveHandler> _logger;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public MealSaveHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, ILogger<MealSaveHandler> logger, IOptionsMonitor<SessionOptions> sessionOptions)
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
        return update.CallbackQuery?.Data is string data && data.StartsWith($"{CallbackPrefixConstants.MealSave}{CallbackPrefixConstants.Separator}");
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var data = update.CallbackQuery!.Data!;
        var parts = data.Split(CallbackPrefixConstants.Separator);

        if (parts.Length < 3 || !Guid.TryParse(parts[2], out var analysisResultId))
        {
            _logger.LogDebug($"Invalid meal save callback data: {data}");
            return;
        }

        var result = await _mediator.Send(new SaveMealMessage(chatId, analysisResultId), cancellationToken);
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
