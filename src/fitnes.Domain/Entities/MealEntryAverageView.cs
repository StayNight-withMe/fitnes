using fitnes.Domain.Enums;

namespace fitnes.Domain.Entities;

public class MealEntryAverageView
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public DateTime ConsumedAt { get; set; }
    public EntrySource Source { get; set; }
    public string? DishName { get; set; }
    public double ConfidenceScore { get; set; }
    public double? AvgCalories { get; set; }
    public double? AvgProtein { get; set; }
    public double? AvgFat { get; set; }
    public double? AvgCarbs { get; set; }
}
