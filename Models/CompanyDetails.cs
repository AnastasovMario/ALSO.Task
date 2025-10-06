namespace ALSO;

public class CompanyDetails
{
    public string Company { get; init; } = default!;
    public string Login { get; init; } = default!;
    public Contact Contact { get; init; } = default!;
    public List<LicenseItem> Licenses { get; init; } = new();
}

