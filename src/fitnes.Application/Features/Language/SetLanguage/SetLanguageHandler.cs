using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Application.Features.Start;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Domain.Models.Common;
using fitnes.Domain.Options;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace fitnes.Application.Features.Language.SetLanguage;

public class SetLanguageHandler : IRequestHandler<SetLanguageMessage, Result<WorkFlowResponse>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IBaseRepository<User, long> _userRepostory;
    private readonly IRequestContext _context;
    private readonly IMediator _mediator;
    private readonly ILogger<SetLanguageHandler> _logger;
    private readonly IOptions<SessionOptions> _sessionOptions;

    public SetLanguageHandler(IBaseRepository<User, long> userRepostory, 
        ISessionRepository sessionRepository, 
        IRequestContext context, 
        IMediator mediator, 
        ILogger<SetLanguageHandler> logger,
        IOptions<SessionOptions> sessionOptions)
    {
        _userRepostory = userRepostory;
        _sessionRepository = sessionRepository;
        _context = context;
        _mediator = mediator;
        _logger = logger;
        _sessionOptions = sessionOptions;
    }

    public async Task<Result<WorkFlowResponse>> Handle(SetLanguageMessage request, CancellationToken cancellationToken)
    {
        var session = _context.Session;
        var currentLanguage = session.Language;
        
        if (request.Language != currentLanguage)
        {
            session.Language = request.Language;
            
            _logger.LogDebug("Update language in database and session");
            
            var user = await _userRepostory.GetById(request.ChatId, cancellationToken);
            if(user is not null)
            {
                user.Language = request.Language;
                await _userRepostory.Update(user, cancellationToken);
            }

            await _sessionRepository.SaveSession(session, TimeSpan.FromMinutes(_sessionOptions.Value.ExpiryMinutes), cancellationToken);
        }
        
        return await _mediator.Send(new Start.StartMessage(session.Id));
    }
}
