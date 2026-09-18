using System.Globalization;
using fitnes.Application.Features.Goals.New;
using fitnes.Application.Features.Goals.SetActivity;
using fitnes.Application.Features.Goals.SetType;
using fitnes.Application.Features.Goals.View;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Goals;

public class GoalActionHandler : IBotHandler
{
    private readonly IMediator _mediator;
    private readonly ISessionRepository _sessionRepository;
    private readonly IRequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<GoalActionHandler> _logger;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public GoalActionHandler(IMediator mediator, ISessionRepository sessionRepository, IRequestContext context, ITelegramBotClient botClient, ILogger<GoalActionHandler> logger, IOptionsMonitor<SessionOptions> sessionOptions)
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
        return update.CallbackQuery?.Data is string data && data.StartsWith($"{CallbackPrefixConstants.Goals}{CallbackPrefixConstants.Separator}");
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        var chatId = update.CallbackQuery!.From.Id;
        var data = update.CallbackQuery!.Data!;
        var parts = data.Split(CallbackPrefixConstants.Separator);

        if (parts.Length < 2)
        {
            _logger.LogDebug($"Invalid goal callback data: {data}");
            return;
        }

        if (parts[1].Equals("new"))
        {
            await SendNew(chatId, cancellationToken);
            return;
        }

        if (parts[1].Equals("view"))
        {
            await SendView(chatId, cancellationToken);
            return;
        }

        if (parts[1].Equals("type") && parts.Length > 3 && Enum.TryParse<GoalType>(parts[2], true, out var type) && double.TryParse(parts[3].Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var targetWeight))
        {
            var result = await _mediator.Send(new SetGoalTypeMessage(chatId, type, targetWeight), cancellationToken);
            await SendResult(chatId, result, cancellationToken);
            return;
        }

        if (parts[1].Equals("activity") && parts.Length > 4 && Enum.TryParse<ActivityLevel>(parts[2], true, out var activity) && Enum.TryParse<GoalType>(parts[3], true, out var goalType) && double.TryParse(parts[4].Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var weight))
        {
            var result = await _mediator.Send(new GoalActivityMessage(chatId, activity, goalType, weight), cancellationToken);
            await SendResult(chatId, result, cancellationToken);
            return;
        }

        _logger.LogDebug($"Invalid goal callback data: {data}");
    }

    private async Task SendNew(long chatId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GoalNewMessage(chatId), cancellationToken);
        await SendResult(chatId, result, cancellationToken);
    }

    private async Task SendView(long chatId, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GoalViewMessage(chatId), cancellationToken);
        await SendResult(chatId, result, cancellationToken);
    }

    private async Task SendResult(long chatId, Domain.Models.Common.Result<fitnes.Application.DTOs.WorkFlowResponse.WorkFlowResponse> result, CancellationToken cancellationToken)
    {
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
