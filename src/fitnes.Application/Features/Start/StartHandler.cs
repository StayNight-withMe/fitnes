using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Start;

public class StartHandler : IRequestHandler<StartMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public StartHandler(ILocalizer localizer, IBaseRepository<User, long> userRepository, ISessionService sessionService, IRequestContext context)
    {
        _localizer = localizer;
        _userRepository = userRepository;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(StartMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);
        if (user is null)
        {
            user = User.CreateDefault(request.ChatId);
            await _userRepository.Create(user, cancellationToken);
        }

        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Idle, cancellationToken);
        _context.Session = session;
        var result = new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Idle.Text, user.Language),
            ButtonRows = new[]
            {
                new ButtonRow(
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Idle.BtnStart, user.Language), CallbackPrefixConstants.Work),
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.Idle, LocalizationKeysConstants.Idle.BtnLanguage, user.Language), CallbackPrefixConstants.Language))
            }
        };
        return Result<WorkFlowResponse>.Success(result);
    }
}
