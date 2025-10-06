using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Task = System.Threading.Tasks.Task;

namespace ALSO;

public class ProvisionApi(HttpClient http, IOptions<ApiOptions> opt, ILogger<ProvisionApi> logger) : IProvisionApi
{
    private readonly ApiOptions _options = opt.Value;

    public async Task<IReadOnlyList<Company>> GetCompaniesAsync(string country, CancellationToken ct = default)
    {
        var jsonContent = $"{{\"Country\":\"{country}\"}}";
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await http.PostAsync(_options.GetCompaniesUrl, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("GetCompanies API error: {ErrorContent}", errorContent);
            throw new HttpRequestException(
                $"API returned {(int)response.StatusCode} {response.StatusCode}. Response: {errorContent}");
        }

        var rawResponse = await response.Content.ReadAsStringAsync(ct);
        var trimmedResponse = rawResponse.Trim();

        var responseJson = trimmedResponse.StartsWith("\"") && trimmedResponse.EndsWith("\"")
            ? JsonSerializer.Deserialize<string>(trimmedResponse) ?? trimmedResponse
            : trimmedResponse;

        var data = JsonSerializer.Deserialize<List<Company>>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return data ?? [];
    }

    public async Task<CompanyDetails> GetCompanyDetailsAsync(string companyId, CancellationToken ct = default)
    {
        var jsonContent = $"{{\"CompanyId\":\"{companyId}\"}}";
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await http.PostAsync(_options.GetCompanyDetailsUrl, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("GetCompanyDetails API error: {ErrorContent}", errorContent);
            throw new HttpRequestException(
                $"API returned {(int)response.StatusCode} {response.StatusCode}. Response: {errorContent}");
        }

        var rawResponse = await response.Content.ReadAsStringAsync(ct);
        var trimmedResponse = rawResponse.Trim();

        var responseJson = trimmedResponse.StartsWith("\"") && trimmedResponse.EndsWith("\"")
            ? JsonSerializer.Deserialize<string>(trimmedResponse) ?? trimmedResponse
            : trimmedResponse;

        return JsonSerializer.Deserialize<CompanyDetails>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        })!;
    }

    public async Task<decimal> GetPriceAsync(string sku, CancellationToken ct = default)
    {
        var jsonContent = $"{{\"SKU\":\"{sku}\"}}";
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        var response = await http.PostAsync(_options.GetPriceUrl, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("GetPrice API error: {ErrorContent}", errorContent);
            throw new HttpRequestException(
                $"API returned {(int)response.StatusCode} {response.StatusCode}. Response: {errorContent}");
        }

        var rawResponse = await response.Content.ReadAsStringAsync(ct);
        var trimmedResponse = rawResponse.Trim();

        var responseJson = trimmedResponse.StartsWith("\"") && trimmedResponse.EndsWith("\"")
            ? JsonSerializer.Deserialize<string>(trimmedResponse) ?? trimmedResponse
            : trimmedResponse;

        var price = JsonSerializer.Deserialize<PriceResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return price!.price;
    }

    public async Task SubmitResultAsync(FinalPayload payload, CancellationToken ct = default)
    {
        var payloadJson = JsonSerializer.Serialize(payload);
        var doubleEncodedPayload = JsonSerializer.Serialize(payloadJson);
        var content = new StringContent(doubleEncodedPayload, Encoding.UTF8, "application/json");
        var response = await http.PostAsync(_options.SubmitUrl, content, ct);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(ct);
            logger.LogError("Submit API error: {ErrorContent}", errorContent);
            throw new HttpRequestException(
                $"API returned {(int)response.StatusCode} {response.StatusCode}. Response: {errorContent}");
        }
    }
}

