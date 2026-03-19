namespace WarranTeaApi.Models;

public class Warranty
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

    public string UserId { get; set; } = string.Empty;
    public User User { get; set; } = null!;

    public int? ProductId { get; set; }
    public Product? Product { get; set; }
}
