using fitnes.Domain.Enums;
using fitnes.Domain.Models.Fitnes;

namespace fitnes.Domain.Abstraction.Services;

public interface ICalorieService
{
    public Task<FoodAnalysisResult?> GetCaloriesFromImageAsync(byte[] imageBytes, string? additionalInformation, Language language, CancellationToken cancellationToken);
}
