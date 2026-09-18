using fitnes.bot.Abstractions;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Domain.Options;
using fitnes.Infrastructure.Utils;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Telegram.Bot;
using Telegram.Bot.Types;
using DomainUser = fitnes.Domain.Entities.User;
using TelegramUser = Telegram.Bot.Types.User;

namespace fitnes.bot.Decorators;

public class BotUpdateHandlerDecorator : IBotUpdateHandler
{
    private readonly IBotUpdateHandler _inner;
    private readonly ISessionRepository _sessionRepository;
    private readonly IBaseRepository<DomainUser, long> _userRepository;
    private readonly RequestContext _context;
    private readonly SessionOptions _sessionOptions;
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<BotUpdateHandlerDecorator> _logger;

    public BotUpdateHandlerDecorator(
        IBotUpdateHandler inner,
        ISessionRepository sessionRepository,
        IBaseRepository<DomainUser, long> userRepository,
        RequestContext context,
        IOptions<SessionOptions> sessionOptions,
        ITelegramBotClient botClient,
        ILogger<BotUpdateHandlerDecorator> logger)
    {
        _inner = inner;
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _context = context;
        _sessionOptions = sessionOptions.Value;
        _botClient = botClient;
        _logger = logger;
    }

    public async Task HandleUpdateAsync(Update update, CancellationToken cancellationToken)
    {
        if (update.CallbackQuery is not null)
        {
            await _botClient.AnswerCallbackQuery(update.CallbackQuery.Id, cancellationToken: cancellationToken);
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
                    await _sessionRepository.SaveSession(session, TimeSpan.FromMinutes(_sessionOptions.ExpiryMinutes), cancellationToken);
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
