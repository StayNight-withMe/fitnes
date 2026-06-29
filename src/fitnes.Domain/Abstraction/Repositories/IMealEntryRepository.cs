using fitnes.Domain.Entities;

namespace fitnes.Domain.Abstraction.Repositories;

public interface IMealEntryRepository : IBaseRepository<MealEntry, Guid>
{
    Task<bool> ExistsByAnalysisResultId(Guid analysisResultId, CancellationToken cancellationToken);

    Task<IReadOnlyList<MealEntryAverageView>> GetAverages(long userId, DateTime fromUtc, DateTime toUtc, CancellationToken cancellationToken);
}
