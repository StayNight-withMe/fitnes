using System.Globalization;
using fitnes.Application.DTOs.WorkFlowResponse;
using fitnes.Domain.Abstraction.Common;
using fitnes.Domain.Constants.Bot;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Entities;
using fitnes.Domain.Enums;
using fitnes.Domain.Utils;

namespace fitnes.Application.Features.Goals.Common;

public static class GoalCardBuilder
{
    public static WorkFlowResponse Build(UserGoal goal, User user, ILocalizer localizer)
    {
        var cardTemplate = localizer.GetPhrase(WorkflowStep.Goals, LocalizationKeysConstants.Goals.Card);
        var typeName = localizer.GetPhrase(WorkflowStep.Goals, GoalTypeKey(goal.Type));
        var btnBack = localizer.GetPhrase(WorkflowStep.WorkMenu, LocalizationKeysConstants.WorkMenu.BtnBack);

        var bmr = BodyCompositionCalculator.MifflinBmr(user.Gender, user.Weight, user.Height, user.Age);
        var tdee = bmr * BodyCompositionCalculator.ActivityCoefficient(goal.Activity);
        var targetCalories = (int)BodyCompositionCalculator.TargetCalories(goal.Type, tdee);
        var protein = (int)BodyCompositionCalculator.ProteinNorm(goal.TargetWeight);

        var text = string.Format(
            CultureInfo.InvariantCulture,
            cardTemplate,
            typeName,
            user.Weight,
            goal.TargetWeight,
            targetCalories,
            protein);

        return new WorkFlowResponse
        {
            Text = text,
            ButtonRows = new[]
            {
                new ButtonRow(new ButtonData(btnBack, CallbackPrefixConstants.Back))
            }
        };
    }

    private static string GoalTypeKey(GoalType type)
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
}
