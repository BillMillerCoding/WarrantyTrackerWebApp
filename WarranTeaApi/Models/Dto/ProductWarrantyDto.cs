using System.ComponentModel.DataAnnotations;

namespace WarranTeaApi.Models.Dto;

public class CreateProductWarrantyDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string CoverageType { get; set; } = string.Empty;

    [Required]
    public int DurationMonths { get; set; }

    [Required]
    public string Provider { get; set; } = string.Empty;

    public string? Description { get; set; }
    public string? Url { get; set; }
}

public class UpdateProductWarrantyDto
{
    public string? CoverageType { get; set; }
    public int? DurationMonths { get; set; }
    public string? Provider { get; set; }
    public string? Description { get; set; }
    public string? Url { get; set; }
}
