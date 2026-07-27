using fitnes.Domain.Entities;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Options;
using Microsoft.Extensions.Options;
using fitnes.Domain.Enums;
using fitnes.Domain.Utils;
using fitnes.Domain.Constants.Localization;
using MediatR;
using fitnes.Domain.Abstraction.Bot;
using fitnes.Domain.Models.Common;
using fitnes.Infrastructure.Utils;
using fitnes.Domain.Abstraction.Common;

namespace fitnes.Infrastructure.Behaviors;

public class SessionBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
    where TRequest : notnull
    where TResponse : Result
{
    private readonly RequestContext _context; 
    private readonly ISessionRepository _sessionRepository;
    private readonly ILocalizer _localizer;
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly SessionOptions _sessionOptions;

    public SessionBehavior(RequestContext context,
        ISessionRepository sessionRepository,
        ILocalizer localizer,
        IBaseRepository<User, long> userRepository,
        IOptions<SessionOptions> sessionOptions)
    {
        _context = context;
        _sessionRepository = sessionRepository;
        _localizer = localizer;
        _userRepository = userRepository;
        _sessionOptions = sessionOptions.Value;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        if(request is IAuthorizedBotRequest botRequest)
        {
            if (_context.Session is not null)
            {
                return await next(cancellationToken);
            }

            var session = await _sessionRepository.GetSession(botRequest.ChatId, cancellationToken);

            if (session is null)
            {
                var user = await _userRepository.GetById(botRequest.ChatId, cancellationToken);
                if (user is null)
                {
                    var errorText = _localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Errors.SessionExpired, LocalizationConstants.DefaultLanguage);
                    return ResultFactory.Failure<TResponse>(Errors.SessionExpired, [ errorText ]);
                }

                session = new UserSession
                {
                    Id = user.Id,
                    Language = user.Language,
                    State = WorkflowStep.Idle,
                    LastUpdate = DateTime.UtcNow
                };

                await _sessionRepository.SaveSession(session, TimeSpan.FromMinutes(_sessionOptions.ExpiryMinutes), cancellationToken);
            }

            _context.Session = session;
        }
        return await next(cancellationToken);
    }
}
