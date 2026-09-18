using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Goals;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;

namespace fitnes.Application.Features.Goals.Common;

public static class GoalCoherence
{
    public static bool IsMismatch(GoalType type, double currentWeight, double targetWeight)
    {
        if (type is GoalType.LoseWeight)
        {
            return targetWeight >= currentWeight;
        }

        if (type is GoalType.GainWeight)
        {
            return targetWeight <= currentWeight;
        }

        return Math.Abs(targetWeight - currentWeight) > GoalValidationConstants.MaintainToleranceKg;
    }

    public static GoalType ResolveImpliedType(double currentWeight, double targetWeight)
    {
        if (targetWeight < currentWeight)
        {
            return GoalType.LoseWeight;
        }

        if (targetWeight > currentWeight)
        {
            return GoalType.GainWeight;
        }

        return GoalType.Maintain;
    }

    public static string GoalTypeKey(GoalType type)
    {
        if (type is GoalType.GainWeight)
        {
            return LocalizationKeysConstants.Goals.BtnGain;
        }

        if (type is GoalType.Maintain)
        {
            return LocalizationKeysConstants.Goals.BtnMaintain;
        }

        return LocalizationKeysConstants.Goals.BtnLose;
    }

    public static WorkFlowResponse BuildMismatchResponse(GoalType chosenType, double currentWeight, double targetWeight, ILocalizer localizer)
    {
        var implied = ResolveImpliedType(currentWeight, targetWeight);
        var template = localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.Mismatch);
        var chosenName = localizer.GetPhrase(WorkflowStep.Goals, GoalTypeKey(chosenType));
        var impliedName = localizer.GetPhrase(WorkflowStep.Goals, GoalTypeKey(implied));
        var btnBack = localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);
        var weightValue = targetWeight.ToString(CultureInfo.InvariantCulture);

        return new WorkFlowResponse
        {
            Text = string.Format(CultureInfo.InvariantCulture, template, chosenName, targetWeight, currentWeight, impliedName),
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(impliedName, $"{CallbackPrefixConstants.GoalTypePrefix}{CallbackPrefixConstants.Separator}{implied}{CallbackPrefixConstants.Separator}{weightValue}")),
                new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
            }
        };
    }
}
