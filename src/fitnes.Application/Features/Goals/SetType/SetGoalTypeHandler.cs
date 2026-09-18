using System.Globalization;
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

namespace fitnes.Application.Features.Goals.SetType;

public class SetGoalTypeHandler : IRequestHandler<SetGoalTypeMessage, Result<WorkFlowResponse>>
{
    private readonly IBaseRepository<User, long> _userRepository;
    private readonly ILocalizer _localizer;

    public SetGoalTypeHandler(IBaseRepository<User, long> userRepository, ILocalizer localizer)
    {
        _userRepository = userRepository;
        _localizer = localizer;
    }

    public async Task<Result<WorkFlowResponse>> Handle(SetGoalTypeMessage request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetById(request.ChatId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<WorkFlowResponse>(Errors.UserNotFound);
        }

        if (GoalCoherence.IsMismatch(request.Type, user.Weight, request.TargetWeight))
        {
            return Result<WorkFlowResponse>.Success(GoalCoherence.BuildMismatchResponse(request.Type, user.Weight, request.TargetWeight, _localizer));
        }

        return Result<WorkFlowResponse>.Success(BuildActivityPicker(request));
    }

    private WorkFlowResponse BuildActivityPicker(SetGoalTypeMessage request)
    {
        var askText = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.AskActivity);
        var lines = new[]
        {
            ActivityLine(ActivityLevel.Sedentary, LocalizationKeysConstants.Goals.ActivitySedentary, LocalizationKeysConstants.Goals.ActivitySedentaryDesc),
            ActivityLine(ActivityLevel.Light, LocalizationKeysConstants.Goals.ActivityLight, LocalizationKeysConstants.Goals.ActivityLightDesc),
            ActivityLine(ActivityLevel.Moderate, LocalizationKeysConstants.Goals.ActivityModerate, LocalizationKeysConstants.Goals.ActivityModerateDesc),
            ActivityLine(ActivityLevel.Active, LocalizationKeysConstants.Goals.ActivityActive, LocalizationKeysConstants.Goals.ActivityActiveDesc),
            ActivityLine(ActivityLevel.VeryActive, LocalizationKeysConstants.Goals.ActivityVeryActive, LocalizationKeysConstants.Goals.ActivityVeryActiveDesc)
        };

        var weightValue = request.TargetWeight.ToString(CultureInfo.InvariantCulture);
        var btnBack = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

        return new WorkFlowResponse
        {
            Text = askText + "\n" + string.Join("\n", lines),
            ButtonRows = new[]
            {
                new ButtonRow(ActivityButton(ActivityLevel.Sedentary, request.Type, weightValue)),
                new ButtonRow(ActivityButton(ActivityLevel.Light, request.Type, weightValue)),
                new ButtonRow(ActivityButton(ActivityLevel.Moderate, request.Type, weightValue)),
                new ButtonRow(ActivityButton(ActivityLevel.Active, request.Type, weightValue)),
                new ButtonRow(ActivityButton(ActivityLevel.VeryActive, request.Type, weightValue)),
                new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
            }
        };
    }

    private string ActivityLine(ActivityLevel level, string nameKey, string descKey)
    {
        var name = _localizer.GetPhrase(WorkflowStep.Goals, nameKey);
        var desc = _localizer.GetPhrase(WorkflowStep.Goals, descKey);
        return $"{name} — {desc}";
    }

    private ButtonData ActivityButton(ActivityLevel level, GoalType type, string weightValue)
    {
        var name = _localizer.GetPhrase(WorkflowStep.Goals, ActivityNameKey(level));
        var callback = $"{CallbackPrefixConstants.GoalActivityPrefix}{CallbackPrefixConstants.Separator}{level}{CallbackPrefixConstants.Separator}{type}{CallbackPrefixConstants.Separator}{weightValue}";
        return new ButtonData(name, callback);
    }

    private static string ActivityNameKey(ActivityLevel level)
    {
        if (level is ActivityLevel.Light)
        {
            return LocalizationKeysConstants.Goals.ActivityLight;
        }

        if (level is ActivityLevel.Moderate)
        {
            return LocalizationKeysConstants.Goals.ActivityModerate;
        }

        if (level is ActivityLevel.Active)
        {
            return LocalizationKeysConstants.Goals.ActivityActive;
        }

        if (level is ActivityLevel.VeryActive)
        {
            return LocalizationKeysConstants.Goals.ActivityVeryActive;
        }

        return LocalizationKeysConstants.Goals.ActivitySedentary;
    }
}
