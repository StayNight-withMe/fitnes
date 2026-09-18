namespace fitnes.Domain.Models.Fitnes;

using System.Text.Json.Serialization;
using fitnes.Domain.Abstraction.Repositories;

public class FoodAnalysisResult : IEntity<Guid>
{
    [JsonIgnore]
    public Guid Id { get; set; }

    [JsonPropertyName("dish_name")]
    public string? Dish_Name { get; set; }

    [JsonPropertyName("confidence_score")]
    public double Confidence_Score { get; set; }

    [JsonPropertyName("nutrition")]
    public Nutrition? Nutrition { get; set; }

    [JsonPropertyName("serving")]
    public Serving? Serving { get; set; }

    [JsonPropertyName("analysis_summary")]
    public string? Analysis_Summary { get; set; }
}
