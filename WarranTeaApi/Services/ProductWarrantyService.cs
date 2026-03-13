using Microsoft.EntityFrameworkCore;
using WarranTeaApi.Data;
using WarranTeaApi.Models;
using WarranTeaApi.Models.Dto;

namespace WarranTeaApi.Services;

public class ProductWarrantyService
{
    private readonly AppDbContext _db;
    private readonly SearchService _searchService;

    public ProductWarrantyService(AppDbContext db, SearchService searchService)
    {
        _db = db;
        _searchService = searchService;
    }

    public async Task<List<ProductWarranty>> GetAllAsync()
    {
        return await _db.ProductWarranties
            .Include(pw => pw.Product)
            .ToListAsync();
    }

    public async Task<ProductWarranty?> GetByIdAsync(int id)
    {
        return await _db.ProductWarranties
            .Include(pw => pw.Product)
            .FirstOrDefaultAsync(pw => pw.Id == id);
    }

    public async Task<ProductWarranty?> CreateAsync(CreateProductWarrantyDto dto)
    {
        var product = await _db.Products.FindAsync(dto.ProductId);
        if (product == null)
            return null;

        var pw = new ProductWarranty
        {
            ProductId = dto.ProductId,
            CoverageType = dto.CoverageType,
            DurationMonths = dto.DurationMonths,
            Provider = dto.Provider,
            Description = dto.Description,
            Url = dto.Url
        };

        _db.ProductWarranties.Add(pw);
        await _db.SaveChangesAsync();

        pw.Product = product;
        await _searchService.IndexDocumentAsync(pw);

        return pw;
    }

    public async Task<ProductWarranty?> UpdateAsync(int id, UpdateProductWarrantyDto dto)
    {
        var pw = await _db.ProductWarranties
            .Include(pw => pw.Product)
            .FirstOrDefaultAsync(pw => pw.Id == id);

        if (pw == null)
            return null;

        if (dto.CoverageType != null) pw.CoverageType = dto.CoverageType;
        if (dto.DurationMonths.HasValue) pw.DurationMonths = dto.DurationMonths.Value;
        if (dto.Provider != null) pw.Provider = dto.Provider;
        if (dto.Description != null) pw.Description = dto.Description;
        if (dto.Url != null) pw.Url = dto.Url;

        await _db.SaveChangesAsync();
        await _searchService.IndexDocumentAsync(pw);

        return pw;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var pw = await _db.ProductWarranties.FindAsync(id);
        if (pw == null)
            return false;

        _db.ProductWarranties.Remove(pw);
        await _db.SaveChangesAsync();
        await _searchService.DeleteDocumentAsync(id);

        return true;
    }

    public async Task<int> ReindexAllAsync()
    {
        await _searchService.CreateIndexIfNotExistsAsync();

        var all = await _db.ProductWarranties
            .Include(pw => pw.Product)
            .ToListAsync();

        if (all.Count > 0)
            await _searchService.IndexDocumentsAsync(all);

        return all.Count;
    }
}
