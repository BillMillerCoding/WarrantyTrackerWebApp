namespace WarranTeaApi.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Brand { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<ProductWarranty> ProductWarranties { get; set; } = new List<ProductWarranty>();
    public ICollection<Warranty> Warranties { get; set; } = new List<Warranty>();
}
