using System.ComponentModel.DataAnnotations;

namespace WarranTeaApi.Models.Dto;

public class CreateWarrantyDto
{
    [Required]
    public string ProductName { get; set; } = string.Empty;

    [Required]
    public string Brand { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    [Required]
    public DateTime PurchaseDate { get; set; }

    [Required]
    public DateTime ExpirationDate { get; set; }

    [Required]
    public string CoverageType { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public int? ProductId { get; set; }
}

public class UpdateWarrantyDto
{
    public string? ProductName { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? CoverageType { get; set; }
    public string? Notes { get; set; }
    public int? ProductId { get; set; }
}

public class WarrantyResponseDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; }
    public DateTime ExpirationDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string CoverageType { get; set; } = string.Empty;
    public string? ReceiptUrl { get; set; }
    public string? WarrantyDocUrl { get; set; }
    public string? Notes { get; set; }
    public int? ProductId { get; set; }
}
