using fitnes.Application.Features.Meals.MealStats;
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

namespace fitnes.bot.Handlers.Meals;

public class MealStatsHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<MealStatsHandler> _logger;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public MealStatsHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, ILogger<MealStatsHandler> logger, IOptionsMonitor<SessionOptions> sessionOptions)
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
        return update.CallbackQuery?.Data is string data && (data.Equals(CallbackPrefixConstants.Stats) || data.StartsWith($"{CallbackPrefixConstants.Stats}{CallbackPrefixConstants.Separator}"));
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var data = update.CallbackQuery!.Data!;
        var parts = data.Split(CallbackPrefixConstants.Separator);

        var granularity = StatsGranularity.Day;
        var offset = 0;

        if (parts.Length > 2)
        {
            if (!Enum.TryParse<StatsGranularity>(parts[1], true, out granularity))
            {
                _logger.LogDebug($"Invalid stats granularity: {data}");
                return;
            }

            if (!int.TryParse(parts[2], out offset))
            {
                _logger.LogDebug($"Invalid stats offset: {data}");
                return;
            }

            if (offset > 0)
            {
                offset = 0;
            }
        }

        var result = await _mediator.Send(new MealStatsMessage(chatId, granularity, offset), cancellationToken);
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
