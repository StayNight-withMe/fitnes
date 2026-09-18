using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;
using Microsoft.Extensions.Logging;

namespace fitnes.Application.Features.StartWork;

public class StartWorkHadner : IRequestHandler<StartWorkMessage, Result<WorkFlowResponse>>
{
    private readonly ILogger<StartWorkHadner> _logger;
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public StartWorkHadner(ILogger<StartWorkHadner> logger, ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _logger = logger;
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(StartWorkMessage request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.WorkMenu, cancellationToken);
        _context.Session = session;
        var result = new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.Text),
            ButtonRows = new[]
            {
                new ButtonRow(
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnGoals), CallbackPrefixConstants.Goals)),
                new ButtonRow(
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnProfile), CallbackPrefixConstants.Profile)),
                new ButtonRow(
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCalories), CallbackPrefixConstants.Calories)),
                new ButtonRow(
                    new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnNutrition), CallbackPrefixConstants.Stats))
            }
        };
        return Result<WorkFlowResponse>.Success(result);
    }
}
