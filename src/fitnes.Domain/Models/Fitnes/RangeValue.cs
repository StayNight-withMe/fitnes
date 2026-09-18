namespace fitnes.Domain.Models.Fitnes;

using System.Text.Json.Serialization;

public class RangeValue
{
    [JsonPropertyName("min")]
    public double Min { get; set; }

    [JsonPropertyName("max")]
    public double Max { get; set; }
}
