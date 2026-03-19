using System.Text;
using Microsoft.EntityFrameworkCore;
using WarranTeaApi.Data;
using WarranTeaApi.Models;
using WarranTeaApi.Models.Dto;

namespace WarranTeaApi.Services;

public class WarrantyService
{
    private readonly AppDbContext _db;
    private readonly BlobService _blobService;
    private readonly EmbeddingService _embeddingService;

    public WarrantyService(AppDbContext db, BlobService blobService, EmbeddingService embeddingService)
    {
        _db = db;
        _blobService = blobService;
        _embeddingService = embeddingService;
    }

    public static string ComputeStatus(DateTime expirationDate)
    {
        var daysLeft = (expirationDate - DateTime.UtcNow).TotalDays;
        if (daysLeft <= 0) return "expired";
        if (daysLeft <= 30) return "expiring-soon";
        return "active";
    }

    public static WarrantyResponseDto ToDto(Warranty w) => new()
    {
        Id = w.Id,
        ProductName = w.ProductName,
        Brand = w.Brand,
        Category = w.Category,
        PurchaseDate = w.PurchaseDate,
        ExpirationDate = w.ExpirationDate,
        Status = ComputeStatus(w.ExpirationDate),
        CoverageType = w.CoverageType,
        ReceiptUrl = w.ReceiptUrl,
        WarrantyDocUrl = w.WarrantyDocUrl,
        Notes = w.Notes,
        ProductId = w.ProductId
    };

    public async Task<List<WarrantyResponseDto>> GetAllAsync(string userId, string? query, string? brand, string? status)
    {
        var q = _db.Warranties.Where(w => w.UserId == userId);

        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(w => w.ProductName.Contains(query) || w.Brand.Contains(query) || w.Category.Contains(query));

        if (!string.IsNullOrWhiteSpace(brand))
            q = q.Where(w => w.Brand == brand);

        var warranties = await q.OrderByDescending(w => w.PurchaseDate).ToListAsync();
        var dtos = warranties.Select(ToDto).ToList();

        if (!string.IsNullOrWhiteSpace(status))
            dtos = dtos.Where(d => d.Status == status).ToList();

        return dtos;
    }

    public async Task<WarrantyResponseDto?> GetByIdAsync(string userId, int id)
    {
        var warranty = await _db.Warranties.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        return warranty == null ? null : ToDto(warranty);
    }

    public async Task<WarrantyResponseDto> CreateAsync(string userId, CreateWarrantyDto dto, Stream? fileStream, string? fileName, string? contentType)
    {
        string? receiptUrl = null;
        string? warrantyDocUrl = null;

        if (fileStream != null)
        {
            // Upload the receipt image to blob storage
            receiptUrl = await _blobService.UploadAsync(fileStream, fileName!, contentType!);

            // OCR the receipt: extract text, store as .txt in blob, save URL
            fileStream.Position = 0;
            var extractedText = await _embeddingService.ExtractTextAsync(fileStream, contentType!);

            if (!string.IsNullOrWhiteSpace(extractedText))
            {
                var textBytes = Encoding.UTF8.GetBytes(extractedText);
                using var textStream = new MemoryStream(textBytes);
                var textFileName = Path.GetFileNameWithoutExtension(fileName) + "_ocr.txt";
                warrantyDocUrl = await _blobService.UploadAsync(textStream, textFileName, "text/plain");
            }
        }

        var warranty = new Warranty
        {
            ProductName = dto.ProductName,
            Brand = dto.Brand,
            Category = dto.Category,
            PurchaseDate = dto.PurchaseDate,
            ExpirationDate = dto.ExpirationDate,
            Status = ComputeStatus(dto.ExpirationDate),
            CoverageType = dto.CoverageType,
            Notes = dto.Notes,
            ReceiptUrl = receiptUrl,
            WarrantyDocUrl = warrantyDocUrl,
            ProductId = dto.ProductId,
            UserId = userId
        };

        _db.Warranties.Add(warranty);
        await _db.SaveChangesAsync();

        return ToDto(warranty);
    }

    public async Task<WarrantyResponseDto?> UpdateAsync(string userId, int id, UpdateWarrantyDto dto)
    {
        var warranty = await _db.Warranties.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (warranty == null)
            return null;

        if (dto.ProductName != null) warranty.ProductName = dto.ProductName;
        if (dto.Brand != null) warranty.Brand = dto.Brand;
        if (dto.Category != null) warranty.Category = dto.Category;
        if (dto.PurchaseDate.HasValue) warranty.PurchaseDate = dto.PurchaseDate.Value;
        if (dto.ExpirationDate.HasValue) warranty.ExpirationDate = dto.ExpirationDate.Value;
        if (dto.CoverageType != null) warranty.CoverageType = dto.CoverageType;
        if (dto.Notes != null) warranty.Notes = dto.Notes;
        if (dto.ProductId.HasValue) warranty.ProductId = dto.ProductId;

        warranty.Status = ComputeStatus(warranty.ExpirationDate);

        await _db.SaveChangesAsync();
        return ToDto(warranty);
    }

    public async Task<bool> DeleteAsync(string userId, int id)
    {
        var warranty = await _db.Warranties.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (warranty == null)
            return false;

        if (!string.IsNullOrEmpty(warranty.ReceiptUrl))
            await _blobService.DeleteAsync(warranty.ReceiptUrl);

        if (!string.IsNullOrEmpty(warranty.WarrantyDocUrl))
            await _blobService.DeleteAsync(warranty.WarrantyDocUrl);

        _db.Warranties.Remove(warranty);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<WarrantyResponseDto?> UploadReceiptAsync(string userId, int id, Stream fileStream, string fileName, string contentType)
    {
        var warranty = await _db.Warranties.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (warranty == null)
            return null;

        if (!string.IsNullOrEmpty(warranty.ReceiptUrl))
            await _blobService.DeleteAsync(warranty.ReceiptUrl);
        if (!string.IsNullOrEmpty(warranty.WarrantyDocUrl))
            await _blobService.DeleteAsync(warranty.WarrantyDocUrl);

        warranty.ReceiptUrl = await _blobService.UploadAsync(fileStream, fileName, contentType);

        // OCR the receipt and store extracted text as a blob
        fileStream.Position = 0;
        var extractedText = await _embeddingService.ExtractTextAsync(fileStream, contentType);
        if (!string.IsNullOrWhiteSpace(extractedText))
        {
            var textBytes = Encoding.UTF8.GetBytes(extractedText);
            using var textStream = new MemoryStream(textBytes);
            var textFileName = Path.GetFileNameWithoutExtension(fileName) + "_ocr.txt";
            warranty.WarrantyDocUrl = await _blobService.UploadAsync(textStream, textFileName, "text/plain");
        }

        await _db.SaveChangesAsync();
        return ToDto(warranty);
    }

    public async Task<WarrantyResponseDto?> UploadWarrantyDocAsync(string userId, int id, Stream fileStream, string fileName, string contentType)
    {
        var warranty = await _db.Warranties.FirstOrDefaultAsync(w => w.Id == id && w.UserId == userId);
        if (warranty == null)
            return null;

        if (!string.IsNullOrEmpty(warranty.WarrantyDocUrl))
            await _blobService.DeleteAsync(warranty.WarrantyDocUrl);

        warranty.WarrantyDocUrl = await _blobService.UploadAsync(fileStream, fileName, contentType);
        await _db.SaveChangesAsync();
        return ToDto(warranty);
    }
}
