using fitnes.Domain.Models.Fitnes;

namespace fitnes.Domain.Entities;

using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Enums;

public class MealEntry : IEntity<Guid>
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = default!;
    public DateTime ConsumedAt { get; set; }
    public EntrySource Source { get; set; }
    public Guid? AnalysisResultId { get; set; }
    public FoodAnalysisResult? AnalysisResult { get; set; }
}
