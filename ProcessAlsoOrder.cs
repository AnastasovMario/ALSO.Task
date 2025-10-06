using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Task = System.Threading.Tasks.Task;

namespace ALSO;

public class ProcessAlsoOrder(IProvisionApi api, ILogger<ProcessAlsoOrder> log)
{
    private static readonly string Country = "Latvia";
    private static readonly string TargetCompany = "LIDO";
    private static readonly string[] TargetSkus = ["TPLV7893-85", "TPLV7884-85"];

    [Function("process-order")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", "get")] HttpRequestData req, FunctionContext ctx)
    {
        try
        {
            log.LogInformation("Starting order processing for country: {Country}, company: {Company}", Country, TargetCompany);

            // Step 1: Get companies for Latvia
            var companies = await api.GetCompaniesAsync(Country);
            var company = companies.FirstOrDefault(c => string.Equals(c.CompanyName, TargetCompany, StringComparison.OrdinalIgnoreCase));
            if (company is null)
            {
                log.LogWarning("Company '{Company}' not found in {Country}", TargetCompany, Country);
                return Problem(req, HttpStatusCode.NotFound, $"Company '{TargetCompany}' not found in {Country}.");
            }

            log.LogInformation("Found company: {CompanyId} - {CompanyName}", company.CompanyId, company.CompanyName);

            // Step 2: Get company details
            var details = await api.GetCompanyDetailsAsync(company.CompanyId);
            log.LogInformation("Retrieved company details for login: {Login}", details.Login);

            // Step 3: Process target SKUs and get prices
            var lines = new List<OrderedLicenseLine>();
            foreach (var sku in TargetSkus)
            {
                var license = details.Licenses.FirstOrDefault(l => l.SKU == sku);
                if (license is null)
                {
                    log.LogWarning("SKU {SKU} not found in company licenses", sku);
                    continue;
                }

                var unitPrice = await api.GetPriceAsync(sku);
                var price = Money.Round2(unitPrice);
                var sum = Money.Round2(price * license.count);

                log.LogInformation("Processed SKU {SKU}: Price={Price}, Count={Count}, Sum={Sum}", 
                    sku, price, license.count, sum);

                lines.Add(new OrderedLicenseLine
                {
                    SKU = sku,
                    Price = price,
                    Count = license.count,
                    Sum = sum
                });
            }

            if (lines.Count == 0)
            {
                log.LogWarning("No target SKUs found in company licenses");
                return Problem(req, HttpStatusCode.NotFound, "No target SKUs found in company licenses.");
            }

            // Step 4: Build final payload
            var payload = new FinalPayload
            {
                CompanyId = company.CompanyId,
                CompanyName = company.CompanyName,
                UserLogin = details.Login,
                UserName = $"{details.Contact.Name} {details.Contact.Name}".Trim(),
                OrderedLicense = lines
            };

            log.LogInformation("Built final payload for {UserName} with {LicenseCount} licenses", 
                payload.UserName, payload.OrderedLicense.Count);

            // Step 5: Submit result
            await api.SubmitResultAsync(payload);
            log.LogInformation("Successfully submitted order payload");

            // Return success response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(payload);
            return response;
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Processing failed: {Message}", ex.Message);
            return Problem(req, HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    private static HttpResponseData Problem(HttpRequestData req, HttpStatusCode code, string detail)
    {
        var response = req.CreateResponse(code);
        response.WriteAsJsonAsync(new
        {
            title = "Processing error",
            status = (int)code,
            detail
        }).GetAwaiter().GetResult();
        return response;
    }
}
