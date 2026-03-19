using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public class ProductServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProducts(db);
        var service = new ProductService(db);

        var results = await service.GetAllAsync();

        Assert.Equal(2, results.Count);
    }

    [Fact]
    public async Task GetAllAsync_EmptyDb_ReturnsEmpty()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductService(db);

        var results = await service.GetAllAsync();

        Assert.Empty(results);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingProduct_ReturnsWithWarranties()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProductWarranties(db);
        var service = new ProductService(db);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal("MacBook Pro", result.Name);
        Assert.Single(result.ProductWarranties);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistent_ReturnsNull()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        var service = new ProductService(db);

        var result = await service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetByIdAsync_ProductWithoutWarranties_ReturnsEmptyCollection()
    {
        using var db = TestDbHelper.CreateInMemoryContext();
        TestDbHelper.SeedProducts(db);
        var service = new ProductService(db);

        var result = await service.GetByIdAsync(1);

        Assert.NotNull(result);
        Assert.Empty(result.ProductWarranties);
    }
}
