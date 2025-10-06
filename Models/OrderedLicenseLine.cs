using System.Text.Json.Serialization;

namespace ALSO;

public class OrderedLicenseLine
{
    [JsonPropertyName("sku")]
    public string SKU { get; init; } = default!;

    [JsonPropertyName("price")]
    public decimal Price { get; init; }

    [JsonPropertyName("count")]
    public int Count { get; init; }

    [JsonPropertyName("sum")]
    public decimal Sum { get; init; }
}

