using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Persistence.Context;
using fitnes.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace fitnes.Infrastructure.Persistence.Repositories.Target;

public class GoalPgRepository : PgBaseRepository<UserGoal, Guid>, IGoalRepository
{
    private readonly DbSet<UserGoal> _goalSet;
    private readonly ApplicationDbContext _dbContext;

    public GoalPgRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _goalSet = applicationDbContext.Set<UserGoal>();
        _dbContext = applicationDbContext;
    }

    public async Task<UserGoal?> GetActiveByUserIdAsync(long userId, CancellationToken cancellationToken)
    {
        return await _goalSet.FirstOrDefaultAsync(e => e.UserId == userId && e.IsActive, cancellationToken);
    }

    public async Task DeactivateActiveAsync(long userId, CancellationToken cancellationToken)
    {
        var active = await GetActiveByUserIdAsync(userId, cancellationToken);

        if (active is not null)
        {
            active.IsActive = false;
            _dbContext.Entry(active).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
