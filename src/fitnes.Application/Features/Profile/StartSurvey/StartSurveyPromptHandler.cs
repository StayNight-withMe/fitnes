using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Profile.StartSurvey;

public class StartSurveyPromptHandler : IRequestHandler<StartSurveyPromptRequest, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public StartSurveyPromptHandler(ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(StartSurveyPromptRequest request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.AwaitingTimezone, cancellationToken);
        _context.Session = session;
        var text = _localizer.GetPhrase(WorkflowStep.AwaitingTimezone, LocalizationKeysConstants.Profile.AskTimezone);
        var response = new WorkFlowResponse
        {
            Text = text,
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel), CallbackPrefixConstants.Cancel))
            }
        };
        return Result<WorkFlowResponse>.Success(response);
    }
}
