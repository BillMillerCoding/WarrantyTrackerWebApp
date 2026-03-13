namespace WarranTeaApi.Models.Dto;

public class ProductWarrantySearchResultDto
{
    public int ProductWarrantyId { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string CoverageType { get; set; } = string.Empty;
    public int DurationMonths { get; set; }
    public string Provider { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? ImageUrl { get; set; }
    public double? Score { get; set; }
}

public class ProductWarrantySearchResponseDto
{
    public List<ProductWarrantySearchResultDto> Results { get; set; } = new();
    public int TotalCount { get; set; }
}
