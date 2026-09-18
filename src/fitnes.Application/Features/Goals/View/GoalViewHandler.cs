using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Application.Features.Goals.Common;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.View;

public class GoalViewHandler : IRequestHandler<GoalViewMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly IGoalRepository _goalRepository;
    private readonly ILocalizer _localizer;

    public GoalViewHandler(IBaseRepository<User, long> userRepository, IGoalRepository goalRepository, ILocalizer localizer)
    {
        _userRepository = userRepository;
        _goalRepository = goalRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(GoalViewMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        var goal = await _goalRepository.GetActiveByUserIdAsync(request.ChatId, cancellationToken);

        if (goal is null)
        {
            var emptyText = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.Empty);
            var btnNew = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnNew);
            var btnBack = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

            return Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = emptyText,
                ButtonRows = new[]
                {
                    new ButtonRow(new ButtonData(btnNew, CallbackPrefixConstants.GoalNew)),
                    new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
                }
            });
        }

        return Result<WorkFlowResponse>.Success(GoalCardBuilder.Build(goal, user, _localizer));
    }
}
