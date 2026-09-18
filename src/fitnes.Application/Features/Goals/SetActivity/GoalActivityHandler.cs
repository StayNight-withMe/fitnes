using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Application.Features.Goals.Common;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.SetActivity;

public class GoalActivityHandler : IRequestHandler<GoalActivityMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly IGoalRepository _goalRepository;
    private readonly ILocalizer _localizer;

    public GoalActivityHandler(IBaseRepository<User, long> userRepository, IGoalRepository goalRepository, ILocalizer localizer)
    {
        _userRepository = userRepository;
        _goalRepository = goalRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(GoalActivityMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        if (user.Weight <= 0 || user.Height <= 0 || user.Age <= 0)
        {
            return Result.Failure<WorkFlowResponse>(Errors.ValidationError);
        }

        if (GoalCoherence.IsMismatch(request.Type, user.Weight, request.TargetWeight))
        {
            return Result<WorkFlowResponse>.Success(GoalCoherence.BuildMismatchResponse(request.Type, user.Weight, request.TargetWeight, _localizer));
        }

        await _goalRepository.DeactivateActiveAsync(request.ChatId, cancellationToken);

        var goal = await _goalRepository.Create(new UserGoal
        {
            UserId = request.ChatId,
            Type = request.Type,
            TargetWeight = request.TargetWeight,
            Activity = request.Activity,
            StartWeight = user.Weight,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }, cancellationToken);

        return Result<WorkFlowResponse>.Success(GoalCardBuilder.Build(goal, user, _localizer));
    }
}
