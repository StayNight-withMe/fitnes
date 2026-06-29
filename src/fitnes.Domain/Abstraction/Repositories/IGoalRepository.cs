using fitnes.Domain.Entities;

namespace fitnes.Domain.Abstraction.Repositories;

public interface IGoalRepository : IBaseRepository<UserGoal, Guid>
{
    Task<UserGoal?> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken);

    Task DeactivateActiveAsync(long userId, CancellationToken cancellationToken);
}
