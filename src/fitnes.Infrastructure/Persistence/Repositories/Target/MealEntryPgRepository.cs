using fitnes.Domain.Abstraction.Repositories;
using fitnes.Domain.Entities;
using fitnes.Infrastructure.Persistence.Context;
using fitnes.Infrastructure.Persistence.Repositories.Base;
using Microsoft.EntityFrameworkCore;

namespace fitnes.Infrastructure.Persistence.Repositories.Target;

public class MealEntryPgRepository : PgBaseRepository<MealEntry, Guid>, IMealEntryRepository
{
    private readonly DbSet<MealEntry> _mealEntrySet;
    private readonly DbSet<MealEntryAverageView> _mealAverageViewSet;

    public MealEntryPgRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
    {
        _mealEntrySet = applicationDbContext.Set<MealEntry>();
        _mealAverageViewSet = applicationDbContext.Set<MealEntryAverageView>();
    }

    public async Task<bool> ExistsByAnalysisResultId(Guid analysisResultId, CancellationToken cancellationToken)
    {
        return await _mealEntrySet.AnyAsync(e => e.AnalysisResultId == analysisResultId, cancellationToken);
    }

    public async Task<IReadOnlyList<MealEntryAverageView>> GetAverages(long userId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken)
    {
        return await _mealAverageViewSet
            .Where(e => e.UserId == userId && e.ConsumedAt >= fromUtc && e.ConsumedAt < toUtc)
            .ToListAsync(cancellationToken);
    }
}
