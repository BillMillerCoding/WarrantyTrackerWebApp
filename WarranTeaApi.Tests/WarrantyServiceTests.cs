using Moq;
using WarranTeaApi.Models.Dto;
using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public class WarrantyServiceTests
{
    private const string UserId = "test-user-1";
    private const string OtherUserId = "test-user-2";

    private static WarrantyService CreateService(Data.AppDbContext db)
    {
        var mockBlob = TestDbHelper.CreateMockBlobService();
        var mockEmbed = TestDbHelper.CreateMockEmbeddingService();
        return new WarrantyService(db, mockBlob.Object, mockEmbed.Object);
    }

    #region ComputeStatus

    [Fact]
    public void ComputeStatus_FutureDate_ReturnsActive()
    {
        var result = WarrantyService.ComputeStatus(DateTime.UtcNow.AddMonths(3));
        Assert.Equal("active", result);
    }

    [Fact]
    public void ComputeStatus_Within30Days_ReturnsExpiringSoon()
    {
        var result = WarrantyService.ComputeStatus(DateTime.UtcNow.AddDays(15));
        Assert.Equal("expiring-soon", result);
    }

    [Fact]
    public void ComputeStatus_PastDate_ReturnsExpired()
    {
        var result = WarrantyService.ComputeStatus(DateTime.UtcNow.AddDays(-1));
        Assert.Equal("expired", result);
    }

    [Fact]
    public void ComputeStatus_Exactly30Days_ReturnsExpiringSoon()
    {
        var result = WarrantyService.ComputeStatus(DateTime.UtcNow.AddDays(30));
        Assert.Equal("expiring-soon", result);
    }

    #endregion

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsOnlyUserWarranties()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var results = await service.GetAllAsync(UserId, null, null, null);

        Assert.Equal(3, results.Count);
    }

    [Fact]
    public async Task GetAllAsync_FilterByQuery_MatchesProductName()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var results = await service.GetAllAsync(UserId, "Laptop", null, null);

        Assert.Single(results);
        Assert.Equal("Laptop", results[0].ProductName);
    }

    [Fact]
    public async Task GetAllAsync_FilterByBrand_MatchesExact()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var results = await service.GetAllAsync(UserId, null, "Dell", null);

        Assert.Single(results);
        Assert.Equal("Dell", results[0].Brand);
    }

    [Fact]
    public async Task GetAllAsync_FilterByStatus_FiltersComputed()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var results = await service.GetAllAsync(UserId, null, null, "expired");

        Assert.Single(results);
        Assert.Equal("expired", results[0].Status);
    }

    [Fact]
    public async Task GetAllAsync_DifferentUser_ReturnsEmpty()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var results = await service.GetAllAsync(OtherUserId, null, null, null);

        Assert.Empty(results);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_ExistingWarranty_ReturnsDto()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.GetByIdAsync(UserId, 1);

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.ProductName);
    }

    [Fact]
    public async Task GetByIdAsync_WrongUser_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.GetByIdAsync(OtherUserId, 1);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = CreateService(db);

        var result = await service.GetByIdAsync(UserId, 999);

        Assert.Null(result);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_WithoutFile_CreatesWarranty()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = CreateService(db);

        var dto = new CreateWarrantyDto
        {
            ProductName = "Tablet",
            Brand = "Apple",
            Category = "Electronics",
            PurchaseDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CoverageType = "Manufacturer",
            Notes = "New tablet"
        };

        var result = await service.CreateAsync(UserId, dto, null, null, null);

        Assert.Equal("Tablet", result.ProductName);
        Assert.Equal("active", result.Status);
        Assert.Null(result.ReceiptUrl);
        Assert.Equal(1, db.Warranties.Count());
    }

    [Fact]
    public async Task CreateAsync_WithFile_UploadsAndSetsReceiptUrl()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var mockBlob = TestDbHelper.CreateMockBlobService();
        mockBlob.Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("https://blob.test/receipt.jpg");
        var mockEmbed = TestDbHelper.CreateMockEmbeddingService();
        mockEmbed.Setup(e => e.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("Extracted OCR text");
        var service = new WarrantyService(db, mockBlob.Object, mockEmbed.Object);

        var dto = new CreateWarrantyDto
        {
            ProductName = "Camera",
            Brand = "Canon",
            Category = "Electronics",
            PurchaseDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(2),
            CoverageType = "Extended"
        };

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.CreateAsync(UserId, dto, stream, "receipt.jpg", "image/jpeg");

        Assert.Equal("https://blob.test/receipt.jpg", result.ReceiptUrl);
        Assert.NotNull(result.WarrantyDocUrl);
        mockBlob.Verify(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_ExistingWarranty_UpdatesFields()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var dto = new UpdateWarrantyDto
        {
            ProductName = "Updated Laptop",
            Brand = "Lenovo"
        };

        var result = await service.UpdateAsync(UserId, 1, dto);

        Assert.NotNull(result);
        Assert.Equal("Updated Laptop", result.ProductName);
        Assert.Equal("Lenovo", result.Brand);
    }

    [Fact]
    public async Task UpdateAsync_WrongUser_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.UpdateAsync(OtherUserId, 1, new UpdateWarrantyDto { ProductName = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_NullFieldsNotUpdated()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.UpdateAsync(UserId, 1, new UpdateWarrantyDto { Notes = "Updated note" });

        Assert.NotNull(result);
        Assert.Equal("Laptop", result.ProductName);
        Assert.Equal("Updated note", result.Notes);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_ExistingWarranty_RemovesAndReturnsTrue()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.DeleteAsync(UserId, 1);

        Assert.True(result);
        Assert.Equal(2, db.Warranties.Count());
    }

    [Fact]
    public async Task DeleteAsync_WrongUser_ReturnsFalse()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedWarranties(db, UserId);
        var service = CreateService(db);

        var result = await service.DeleteAsync(OtherUserId, 1);

        Assert.False(result);
        Assert.Equal(3, db.Warranties.Count());
    }

    [Fact]
    public async Task DeleteAsync_NonExistent_ReturnsFalse()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = CreateService(db);

        var result = await service.DeleteAsync(UserId, 999);

        Assert.False(result);
    }

    [Fact]
    public async Task DeleteAsync_WithBlobUrls_DeletesBlobs()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        db.Warranties.Add(new Models.Warranty
        {
            Id = 10,
            ProductName = "Test",
            Brand = "Test",
            Category = "Test",
            PurchaseDate = DateTime.UtcNow,
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            Status = "active",
            CoverageType = "Test",
            UserId = UserId,
            ReceiptUrl = "https://blob.test/receipt.jpg",
            WarrantyDocUrl = "https://blob.test/doc.txt"
        });
        db.SaveChanges();

        var mockBlob = TestDbHelper.CreateMockBlobService();
        mockBlob.Setup(b => b.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        var mockEmbed = TestDbHelper.CreateMockEmbeddingService();
        var service = new WarrantyService(db, mockBlob.Object, mockEmbed.Object);

        await service.DeleteAsync(UserId, 10);

        mockBlob.Verify(b => b.DeleteAsync("https://blob.test/receipt.jpg"), Times.Once);
        mockBlob.Verify(b => b.DeleteAsync("https://blob.test/doc.txt"), Times.Once);
    }

    #endregion

    #region ToDto

    [Fact]
    public void ToDto_MapsAllFields()
    {
        var warranty = new Models.Warranty
        {
            Id = 42,
            ProductName = "Widget",
            Brand = "Acme",
            Category = "Gadgets",
            PurchaseDate = new DateTime(2025, 1, 1),
            ExpirationDate = DateTime.UtcNow.AddYears(1),
            CoverageType = "Full",
            ReceiptUrl = "https://test.com/receipt",
            WarrantyDocUrl = "https://test.com/doc",
            Notes = "Test notes",
            ProductId = 5
        };

        var dto = WarrantyService.ToDto(warranty);

        Assert.Equal(42, dto.Id);
        Assert.Equal("Widget", dto.ProductName);
        Assert.Equal("Acme", dto.Brand);
        Assert.Equal("Gadgets", dto.Category);
        Assert.Equal("Full", dto.CoverageType);
        Assert.Equal("https://test.com/receipt", dto.ReceiptUrl);
        Assert.Equal("https://test.com/doc", dto.WarrantyDocUrl);
        Assert.Equal("Test notes", dto.Notes);
        Assert.Equal(5, dto.ProductId);
        Assert.Equal("active", dto.Status);
    }

    #endregion
}
