namespace ALSO;

public interface IProvisionApi
{
    Task<IReadOnlyList<Company>> GetCompaniesAsync(string country, CancellationToken ct = default);
    Task<CompanyDetails> GetCompanyDetailsAsync(string companyId, CancellationToken ct = default);
    Task<decimal> GetPriceAsync(string sku, CancellationToken ct = default);
    Task SubmitResultAsync(FinalPayload payload, CancellationToken ct = default);
}

