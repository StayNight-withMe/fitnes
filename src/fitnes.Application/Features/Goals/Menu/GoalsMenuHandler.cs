using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.Menu;

public class GoalsMenuHandler : IRequestHandler<GoalsMenuMessage, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public GoalsMenuHandler(ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(GoalsMenuMessage request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Goals, cancellationToken);
        _context.Session = session;

        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.MenuText),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnNew), CallbackPrefixConstants.GoalNew)),
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnView), CallbackPrefixConstants.GoalView)),
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back))
            }
        });
    }
}
