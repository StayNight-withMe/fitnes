using fitnes.Application.Features.Calories.CalculateCalories;
using fitnes.bot.Abstractions;
using fitnes.bot.Extension.Other;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Enums;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace fitnes.bot.Handlers.Calories;

public class CalorieCalculationHandler : IBotHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IMediator _mediator;
    private readonly IRequestContext _requestContext;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<CalorieCalculationHandler> _logger;
    private readonly IOptionsMonitor<SessionOptions> _sessionOptions;

    public CalorieCalculationHandler(ITelegramBotClient telegramBotClient, IMediator mediator, IRequestContext requestContext, ISessionRepository sessionRepository, ILogger<CalorieCalculationHandler> logger, IOptionsMonitor<SessionOptions> sessionOptions)
    {
        _telegramBotClient = telegramBotClient;
        _mediator = mediator;
        _requestContext = requestContext;
        _sessionRepository = sessionRepository;
        _logger = logger;
        _sessionOptions = sessionOptions;
    }

    public bool CanHandle(Update update)
    {
        return update.Message is { Photo: not null } && _requestContext.Session?.State is WorkflowStep.Calories;
    }

    public async Task HandleAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.Message is { Chat.Id: long chatId, Photo: { } photos })
        {
            string caption = update.Message.Caption ?? string.Empty;
            _logger.LogInformation("Photo received from chat {ChatId}, downloading file {FileId}", chatId, photos.Last().FileId);
            using (MemoryStream memoryStream = new MemoryStream())
            {
                await _telegramBotClient.GetInfoAndDownloadFile(photos.Last().FileId, memoryStream, cancellationToken);
                byte[] imageBytes = memoryStream.ToArray();
                _logger.LogInformation("Photo downloaded for chat {ChatId}, size {ImageBytes} bytes, sending to analysis", chatId, imageBytes.Length);
                var result = await _mediator.Send(new CalculateCaloriesMessage(imageBytes, caption), cancellationToken);
                _logger.LogInformation("Analysis finished for chat {ChatId}, success={Success}", chatId, result.IsSuccess);
                await _telegramBotClient.SendAndDelete(
                    chatId: chatId,
                    text: result.GetTextOrErrors(),
                    replyMarkup: result.ToTelegramMarkup(),
                    sessionRepository: _sessionRepository,
                    context: _requestContext,
                    sessionOptions: _sessionOptions,
                    cancellationToken: cancellationToken
                );
            }
        }
    }
}
