namespace WarranTeaApi.Models;

public class ProductWarranty
{
    public int Id { get; set; }
    public string CoverageType { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }

    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
}
