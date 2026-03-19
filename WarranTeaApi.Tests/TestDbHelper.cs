using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using WarranTeaApi.Data;
using WarranTeaApi.Models;
using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public static class TestDbHelper
{
    public static AppDbContext CreateInMemoryContext(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(dbName ?? Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    public static IConfiguration CreateFakeConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureBlob:ConnectionString"] = "DefaultEndpointsProtocol=https;AccountName=fake;AccountKey=ZmFrZWtleQ==;EndpointSuffix=core.windows.net",
                ["AzureBlob:ContainerName"] = "test",
                ["AzureAIVision:Endpoint"] = "https://fake.cognitiveservices.azure.com",
                ["AzureAIVision:ApiKey"] = "fake-key",
                ["AzureAISearch:Endpoint"] = "https://fake.search.windows.net",
                ["AzureAISearch:AdminApiKey"] = "fake-key",
                ["AzureAISearch:IndexName"] = "test-index"
            })
            .Build();

    public static Mock<BlobService> CreateMockBlobService()
    {
        var mock = new Mock<BlobService>(CreateFakeConfig()) { CallBase = false };
        mock.Setup(b => b.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync("https://blob.test/uploaded.jpg");
        mock.Setup(b => b.DeleteAsync(It.IsAny<string>())).Returns(Task.CompletedTask);
        return mock;
    }

    public static Mock<EmbeddingService> CreateMockEmbeddingService()
    {
        var httpFactory = new Mock<IHttpClientFactory>();
        httpFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(new HttpClient());
        var mock = new Mock<EmbeddingService>(CreateFakeConfig(), httpFactory.Object) { CallBase = false };
        mock.Setup(e => e.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync("Extracted text");
        mock.Setup(e => e.EmbedTextAsync(It.IsAny<string>()))
            .ReturnsAsync(new float[1024]);
        return mock;
    }

    public static Mock<SearchService> CreateMockSearchService()
    {
        var mockEmbed = CreateMockEmbeddingService();
        var mock = new Mock<SearchService>(CreateFakeConfig(), mockEmbed.Object) { CallBase = false };
        mock.Setup(s => s.IndexDocumentAsync(It.IsAny<ProductWarranty>())).Returns(Task.CompletedTask);
        mock.Setup(s => s.IndexDocumentsAsync(It.IsAny<IEnumerable<ProductWarranty>>())).Returns(Task.CompletedTask);
        mock.Setup(s => s.DeleteDocumentAsync(It.IsAny<int>())).Returns(Task.CompletedTask);
        mock.Setup(s => s.CreateIndexIfNotExistsAsync()).Returns(Task.CompletedTask);
        return mock;
    }

    public static void SeedWarranties(AppDbContext db, string userId)
    {
        db.Warranties.AddRange(
            new Warranty
            {
                Id = 1,
                ProductName = "Laptop",
                Brand = "Dell",
                Category = "Electronics",
                PurchaseDate = DateTime.UtcNow.AddMonths(-6),
                ExpirationDate = DateTime.UtcNow.AddMonths(6),
                Status = "active",
                CoverageType = "Manufacturer",
                UserId = userId
            },
            new Warranty
            {
                Id = 2,
                ProductName = "Phone",
                Brand = "Samsung",
                Category = "Electronics",
                PurchaseDate = DateTime.UtcNow.AddYears(-2),
                ExpirationDate = DateTime.UtcNow.AddDays(-10),
                Status = "expired",
                CoverageType = "Extended",
                UserId = userId
            },
            new Warranty
            {
                Id = 3,
                ProductName = "Blender",
                Brand = "KitchenAid",
                Category = "Appliances",
                PurchaseDate = DateTime.UtcNow.AddMonths(-11),
                ExpirationDate = DateTime.UtcNow.AddDays(15),
                Status = "expiring-soon",
                CoverageType = "Manufacturer",
                UserId = userId
            }
        );
        db.SaveChanges();
    }

    public static void SeedProducts(AppDbContext db)
    {
        db.Products.AddRange(
            new Product
            {
                Id = 1,
                Name = "MacBook Pro",
                Brand = "Apple",
                Category = "Electronics",
                Description = "High performance laptop"
            },
            new Product
            {
                Id = 2,
                Name = "Galaxy S24",
                Brand = "Samsung",
                Category = "Electronics",
                Description = "Flagship phone"
            }
        );
        db.SaveChanges();
    }

    public static void SeedProductWarranties(AppDbContext db)
    {
        SeedProducts(db);
        db.ProductWarranties.AddRange(
            new ProductWarranty
            {
                Id = 1,
                ProductId = 1,
                CoverageType = "Manufacturer",
                DurationMonths = 12,
                Provider = "Apple",
                Description = "Standard warranty"
            },
            new ProductWarranty
            {
                Id = 2,
                ProductId = 2,
                CoverageType = "Extended",
                DurationMonths = 24,
                Provider = "Samsung",
                Description = "Extended coverage"
            }
        );
        db.SaveChanges();
    }
}
