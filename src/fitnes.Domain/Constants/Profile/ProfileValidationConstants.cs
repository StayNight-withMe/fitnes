namespace fitnes.Domain.Constants.Profile;

public static class ProfileValidationConstants
{
    public const double WeightMin = 0;
    public const double WeightMax = 500;
    public const double HeightMin = 0;
    public const double HeightMax = 300;
    public const int AgeMin = 0;
    public const int AgeMax = 120;
    public const double MeasurementMin = 0;
    public const double MeasurementMax = 300;
    public const int TimezoneOffsetMinMinutes = -720;
    public const int TimezoneOffsetMaxMinutes = 840;
    public static readonly int[] TimezoneValidMinuteParts = { 0, 15, 30, 45 };
    public const string Comma = ",";
    public const string Dot = ".";
    public const string Colon = ":";
    public const string Plus = "+";
    public const string Minus = "-";
    public const int MinutesPerHour = 60;
}
