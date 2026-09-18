namespace fitnes.Domain.Models.Fitnes;

using System.Text.Json.Serialization;

public class Nutrition
{
    [JsonPropertyName("calories_kcal")]
    public RangeValue? Calories_Kcal { get; set; }

    [JsonPropertyName("protein_g")]
    public RangeValue? Protein_g { get; set; }

    [JsonPropertyName("fat_g")]
    public RangeValue? Fat_g { get; set; }

    [JsonPropertyName("carbs_g")]
    public RangeValue? Carbs_g { get; set; }
}
