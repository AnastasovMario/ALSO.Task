using System.Text.Json.Serialization;

namespace ALSO;

public class FinalPayload
{
    [JsonPropertyName("companyId")]
    public string CompanyId { get; init; } = default!;

    [JsonPropertyName("companyName")]
    public string CompanyName { get; init; } = default!;

    [JsonPropertyName("userLogin")]
    public string UserLogin { get; init; } = default!;

    [JsonPropertyName("userName")]
    public string UserName { get; init; } = default!;

    [JsonPropertyName("orderedLicense")]
    public List<OrderedLicenseLine> OrderedLicense { get; init; } = new();
}

