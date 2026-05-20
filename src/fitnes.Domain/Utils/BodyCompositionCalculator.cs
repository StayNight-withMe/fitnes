using fitnes.Domain.Constants.Nutrition;
using fitnes.Domain.Enums;

namespace fitnes.Domain.Utils;

public static class BodyCompositionCalculator
{
    private const double WeightFactor = 10;
    private const double HeightFactor = 6.25;
    private const double AgeFactor = 5;
    private const double MaleOffset = 5;
    private const double FemaleOffset = -161;

    public static double MifflinBmr(Gender gender, double weightKg, double heightCm, int age)
    {
        var offset = gender is Gender.Female ? FemaleOffset : MaleOffset;
        return WeightFactor * weightKg + HeightFactor * heightCm - AgeFactor * age + offset;
    }

    public static double ActivityCoefficient(ActivityLevel activity)
    {
        if (activity is ActivityLevel.Light)
        {
            return CalorieCalculationConstants.LightCoefficient;
        }

        if (activity is ActivityLevel.Moderate)
        {
            return CalorieCalculationConstants.ModerateCoefficient;
        }

        if (activity is ActivityLevel.Active)
        {
            return CalorieCalculationConstants.ActiveCoefficient;
        }

        if (activity is ActivityLevel.VeryActive)
        {
            return CalorieCalculationConstants.VeryActiveCoefficient;
        }

        return CalorieCalculationConstants.SedentaryCoefficient;
    }

    public static double TargetCalories(GoalType type, double tdee)
    {
        if (type is GoalType.LoseWeight)
        {
            return tdee * CalorieCalculationConstants.LoseWeightRate;
        }

        if (type is GoalType.GainWeight)
        {
            return tdee * CalorieCalculationConstants.GainWeightRate;
        }

        return tdee * CalorieCalculationConstants.MaintainRate;
    }

    public static double ProteinNorm(double targetWeightKg)
    {
        return targetWeightKg * CalorieCalculationConstants.ProteinPerKg;
    }
}
