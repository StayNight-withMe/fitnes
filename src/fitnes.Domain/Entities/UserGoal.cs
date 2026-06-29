using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Enums;

namespace fitnes.Domain.Entities;

public class UserGoal : IEntity<Guid>
{
    public Guid Id { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = default!;
    public GoalType Type { get; set; }
    public double TargetWeight { get; set; }
    public ActivityLevel Activity { get; set; }
    public double StartWeight { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}
