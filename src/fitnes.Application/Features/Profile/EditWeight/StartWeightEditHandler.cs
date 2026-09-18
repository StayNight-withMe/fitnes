using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.EditWeight;

public class StartWeightEditHandler : IRequestHandler<StartWeightEditMessage, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public StartWeightEditHandler(ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(StartWeightEditMessage request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingWeightEdit, cancellationToken);
        _context.Session = session;

        var btnCancel = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel);

        return Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.AwaitingWeight, LocalizationKeysConstants.Profile.AskWeight),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(btnCancel, CallbackPrefixConstants.Cancel))
            }
        });
    }
}
