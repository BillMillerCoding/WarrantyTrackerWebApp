using Moq;
using WarranTeaApi.Models;
using WarranTeaApi.Models.Dto;
using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public class ProductWarrantyServiceTests
{
    private static Mock<SearchService> CreateMockSearch() => TestDbHelper.CreateMockSearchService();

    #region GetAllAsync

    [Fact]
    public async Task GetAllAsync_ReturnsAllWithProducts()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var results = await service.GetAllAsync();

        Assert.Equal(2, results.Count);
        Assert.All(results, pw => Assert.NotNull(pw.Product));
    }

    [Fact]
    public async Task GetAllAsync_EmptyDb_ReturnsEmpty()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var results = await service.GetAllAsync();

        Assert.Empty(results);
    }

    #endregion

    #region GetByIdAsync

    [Fact]
    public async Task GetByIdAsync_Existing_ReturnsWithProduct()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("Manufacturer", result.CoverageType);
        Assert.NotNull(result.Product);
        Assert.Equal("MacBook Pro", result.Product.Name);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    #endregion

    #region CreateAsync

    [Fact]
    public async Task CreateAsync_ValidProduct_CreatesAndIndexes()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProducts(db);
        var mockSearch = CreateMockSearch();
        var service = new ProductWarrantyService(db, mockSearch.Object);

        var dto = new CreateProductWarrantyDto
        {
            ProductId = 1,
            CoverageType = "Extended",
            DurationMonths = 36,
            Provider = "AppleCare",
            Description = "Extended protection"
        };

        var result = await service.CreateAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Extended", result.CoverageType);
        Assert.Equal(36, result.DurationMonths);
        Assert.Equal("AppleCare", result.Provider);
        Assert.NotNull(result.Product);
        mockSearch.Verify(s => s.IndexDocumentAsync(It.IsAny<ProductWarranty>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_InvalidProduct_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var dto = new CreateProductWarrantyDto
        {
            ProductId = 999,
            CoverageType = "Full",
            DurationMonths = 12,
            Provider = "Nobody"
        };

        var result = await service.CreateAsync(dto);

        Assert.Null(result);
    }

    #endregion

    #region UpdateAsync

    [Fact]
    public async Task UpdateAsync_Existing_UpdatesAndReindexes()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var mockSearch = CreateMockSearch();
        var service = new ProductWarrantyService(db, mockSearch.Object);

        var dto = new UpdateProductWarrantyDto
        {
            CoverageType = "Premium",
            DurationMonths = 48
        };

        var result = await service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Premium", result.CoverageType);
        Assert.Equal(48, result.DurationMonths);
        Assert.Equal("Apple", result.Provider); // unchanged
        mockSearch.Verify(s => s.IndexDocumentAsync(It.IsAny<ProductWarranty>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_NonExistent_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var result = await service.UpdateAsync(999, new UpdateProductWarrantyDto { CoverageType = "X" });

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_NullFieldsPreserved()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var result = await service.UpdateAsync(1, new UpdateProductWarrantyDto { Description = "New desc" });

        Assert.NotNull(result);
        Assert.Equal("Manufacturer", result.CoverageType);
        Assert.Equal(12, result.DurationMonths);
        Assert.Equal("New desc", result.Description);
    }

    #endregion

    #region DeleteAsync

    [Fact]
    public async Task DeleteAsync_Existing_RemovesAndDeletesFromIndex()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var mockSearch = CreateMockSearch();
        var service = new ProductWarrantyService(db, mockSearch.Object);

        var result = await service.DeleteAsync(1);

        Assert.True(result);
        Assert.Single(db.ProductWarranties);
        mockSearch.Verify(s => s.DeleteDocumentAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_NonExistent_ReturnsFalse()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductWarrantyService(db, CreateMockSearch().Object);

        var result = await service.DeleteAsync(999);

        Assert.False(result);
    }

    #endregion

    #region ReindexAllAsync

    [Fact]
    public async Task ReindexAllAsync_WithData_ReturnsCountAndIndexes()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var mockSearch = CreateMockSearch();
        var service = new ProductWarrantyService(db, mockSearch.Object);

        var count = await service.ReindexAllAsync();

        Assert.Equal(2, count);
        mockSearch.Verify(s => s.CreateIndexIfNotExistsAsync(), Times.Once);
        mockSearch.Verify(s => s.IndexDocumentsAsync(It.Is<List<ProductWarranty>>(l => l.Count == 2)), Times.Once);
    }

    [Fact]
    public async Task ReindexAllAsync_EmptyDb_ReturnsZero()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var mockSearch = CreateMockSearch();
        var service = new ProductWarrantyService(db, mockSearch.Object);

        var count = await service.ReindexAllAsync();

        Assert.Equal(0, count);
        mockSearch.Verify(s => s.IndexDocumentsAsync(It.IsAny<List<ProductWarranty>>()), Times.Never);
    }

    #endregion
}
