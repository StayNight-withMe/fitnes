using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Calories.CalculateCaloriesInfo;

public class CalculateCaloriesInfoHadnler : IRequestHandler<CalorieCalculationInfoMessage, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;
    private readonly ISessionService _sessionService;
    private readonly IRequestContext _context;

    public CalculateCaloriesInfoHadnler(ILocalizer localizer, ISessionService sessionService, IRequestContext context)
    {
        _localizer = localizer;
        _sessionService = sessionService;
        _context = context;
    }

    public async Task<Result<WorkFlowResponse>> Handle(CalorieCalculationInfoMessage request, CancellationToken cancellationToken)
    {
        var session = await _sessionService.UpsertSession(request.ChatId, WorkflowStep.Calories, cancellationToken);
        _context.Session = session;
        var result = new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.Calories, LocalizationKeysConstants.Calories.Text),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back))
            }
        };
        return Result<WorkFlowResponse>.Success(result);
    }
}
