namespace fitnes.Domain.Enums;

public static class WorkflowStepExtensions
{
    public static bool IsProfileStep(this WorkflowStep step)
    {
        return step is WorkflowStep.AwaitingWeight or WorkflowStep.AwaitingHeight or WorkflowStep.AwaitingAge or WorkflowStep.AwaitingGender or WorkflowStep.AwaitingWaist or WorkflowStep.AwaitingNeck or WorkflowStep.AwaitingHips or WorkflowStep.AwaitingTimezone;
    }
}