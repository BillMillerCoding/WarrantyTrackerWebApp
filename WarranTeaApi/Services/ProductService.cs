using Microsoft.EntityFrameworkCore;
using WarranTeaApi.Data;
using WarranTeaApi.Models;

namespace WarranTeaApi.Services;

public class ProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _db.Products.ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _db.Products
            .Include(p => p.ProductWarranties)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
