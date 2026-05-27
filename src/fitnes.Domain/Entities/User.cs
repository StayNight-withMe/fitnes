using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Constants.Localization;
using fitnes.Domain.Enums;

namespace fitnes.Domain.Entities;

public class User : IEntity<long>
{
    /// <summary>
    /// The unique identifier for the user, corresponding to their Telegram ChatId.
    /// </summary>
    public long Id { get; set; }
    public Language Language { get; set; }
    public double Weight { get; set; }
    public double Height { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }
    public double? WaistCm { get; set; }
    public double? NeckCm { get; set; }
    public double? HipsCm { get; set; }
    public int? TimezoneOffsetMinutes { get; set; }

    //public double DailyCaloriesGoal { get; set; }
    public List<MealEntry> Meals { get; set; } = new();

    public static User CreateDefault(long id)
    {
        return new User
        {
            Id = id,
            Language = LocalizationConstants.DefaultLanguage,
            Gender = Gender.Unknown
        };
    }
}
