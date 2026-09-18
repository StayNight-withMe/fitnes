using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Constants.Profile;
using fitnes.Domain.Enums;
using fitnes.Domain.Models.Common;
using MediatR;

namespace fitnes.Application.Features.Goals.Survey;

public class UpdateGoalHandler : IRequestHandler<UpdateGoalMessage, Result<WorkFlowResponse>>
{
    private readonly ILocalizer _localizer;

    public UpdateGoalHandler(ILocalizer localizer)
    {
        _localizer = localizer;
    }

    public Task<Result<WorkFlowResponse>> Handle(UpdateGoalMessage request, CancellationToken cancellationToken)
    {
        if (!double.TryParse(request.Input.Replace(ProfileValidationConstants.Comma, ProfileValidationConstants.Dot), NumberStyles.Any, CultureInfo.InvariantCulture, out var targetWeight) || targetWeight <= ProfileValidationConstants.WeightMin || targetWeight > ProfileValidationConstants.WeightMax)
        {
            var invalidText = _localizer.GetPhrase(WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Profile.InvalidNumber);
            var askText = _localizer.GetPhrase(WorkflowStep.AwaitingGoalWeight, LocalizationKeysConstants.Goals.AskWeight);
            var btnCancel = _localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnCancel);

            return Task.FromResult(Result<WorkFlowResponse>.Success(new WorkFlowResponse
            {
                Text = invalidText + "\n" + askText,
                ButtonRows = new[]
                {
                    new ButtonRow(new ButtonData(btnCancel, CallbackPrefixConstants.Cancel))
                }
            }));
        }

        var weightValue = targetWeight.ToString(CultureInfo.InvariantCulture);

        return Task.FromResult(Result<WorkFlowResponse>.Success(new WorkFlowResponse
        {
            Text = _localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.TypeText),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnLose), TypeCallback(GoalType.LoseWeight, weightValue))),
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnGain), TypeCallback(GoalType.GainWeight, weightValue))),
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.BtnMaintain), TypeCallback(GoalType.Maintain, weightValue))),
                new ButtonRow(new ButtonData(_localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack), CallbackPrefixConstants.Back))
            }
        }));
    }

    private static string TypeCallback(GoalType type, string weightValue)
    {
        return $"{CallbackPrefixConstants.GoalTypePrefix}{CallbackPrefixConstants.Separator}{type}{CallbackPrefixConstants.Separator}{weightValue}";
    }
}
