using fitnes.bot.Abstractions;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Types;
using DomainUser = fitnes.Domain.Entities.User;
using TelegramUser = Telegram.Bot.Types.User;

namespace fitnes.bot.Decorators;

public class BotUpdateHandlerDecorator : IBotUpdateHandler
{
    private readonly IBotUpdateHandler _inner;
    private readonly ISessionRepository _sessionRepository;
    private readonly ISessionService _sessionService;
    private readonly IBaseRepository<DomainUser, long> _userRepository;
    private readonly RequestContext _context;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<BotUpdateHandlerDecorator> _logger;

    public BotUpdateHandlerDecorator(
        IBotUpdateHandler inner,
        ISessionRepository sessionRepository,
        ISessionService sessionService,
        IBaseRepository<DomainUser, long> userRepository,
        RequestContext context,
        ITelegramBotClient botClient,
        ILogger<BotUpdateHandlerDecorator> logger)
    {
        _inner = inner;
        _sessionRepository = sessionRepository;
        _sessionService = sessionService;
        _userRepository = userRepository;
        _context = context;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.CallbackQuery is not null)
        {
            try
            {
                await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"HanndleUpdate error:{0}", ex);
            }
        }

        long? chatId = null;
        if (update.Message is not null)
        {
            chatId = update.Message.Chat.Id;
        }
        else if (update.CallbackQuery is not null)
        {
            chatId = update.CallbackQuery.From.Id;
        }

        if (chatId.HasValue)
        {
            var session = await _sessionRepository.GetSession(chatId.Value, cancellationToken);
            if (session is null)
            {
                _logger.LogDebug("Сессии не существует");
                var user = await _userRepository.GetById(chatId.Value, cancellationToken);
                if (user is not null)
                {
                    _logger.LogDebug("Пользователь найден, создаю сессию");
                    session = new UserSession
                    {
                        Id = user.Id,
                        Language = user.Language,
                        State = Domain.Enums.WorkflowStep.Idle,
                        LastUpdate = DateTime.UtcNow
                    };
                    await _sessionService.SaveSession(session, cancellationToken);
                }
            }

            if (session is not null)
            {
                _logger.LogDebug($"Сессия сохранена со значением: {session.State}");
                _context.Session = session;
            }
            else
            {
                _logger.LogDebug("Сессия и пользователь отсутствуют, контекст не установлен");
            }
        }

        await _inner.HandleUpdateAsync(update, cancellationToken);
    }
}
