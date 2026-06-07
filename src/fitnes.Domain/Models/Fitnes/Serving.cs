namespace fitnes.Domain.Models.Fitnes;

using System.Text.Json.Serialization;

public class Serving
{
    [JsonPropertyName("value")]
    public double Value { get; set; }

    [JsonPropertyName("unit")]
    public string? Unit { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }
}
