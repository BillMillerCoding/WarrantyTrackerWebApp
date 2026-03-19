using Moq;
using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public class OcrServiceTests
{
    private static OcrService CreateServiceWithText(string text)
    {
        var mockEmbed = TestDbHelper.CreateMockEmbeddingService();
        mockEmbed.Setup(e => e.ExtractTextAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(text);
        return new OcrService(mockEmbed.Object);
    }

    #region ParseWarrantyFields (via ParseWarrantyImageAsync)

    [Fact]
    public async Task ParseWarrantyImageAsync_EmptyText_ReturnsEmptyResult()
    {
        var service = CreateServiceWithText(string.Empty);

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Equal(string.Empty, result.RawText);
        Assert.Null(result.ProductName);
        Assert.Null(result.Brand);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_ExtractsProductName()
    {
        var service = CreateServiceWithText("Product Name: Samsung Galaxy S24\nBrand: Samsung");

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Contains("Samsung Galaxy S24", result.ProductName!);
        Assert.Equal("Samsung", result.Brand);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_ExtractsDates()
    {
        var service = CreateServiceWithText("Purchase Date: 2025-01-15  Expiration Date: 2026-01-15");

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.NotNull(result.PurchaseDate);
        Assert.Equal(new DateTime(2025, 1, 15), result.PurchaseDate);
        Assert.NotNull(result.ExpirationDate);
        Assert.Equal(new DateTime(2026, 1, 15), result.ExpirationDate);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_ExtractsCoverage()
    {
        var service = CreateServiceWithText("Coverage: Extended Protection Plan  Category: Electronics");

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Equal("Extended Protection Plan", result.CoverageType);
        Assert.Equal("Electronics", result.Category);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_NoMatchingFields_ReturnsRawTextOnly()
    {
        var text = "This is just random text with no warranty info.";
        var service = CreateServiceWithText(text);

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Equal(text, result.RawText);
        Assert.Null(result.ProductName);
        Assert.Null(result.Brand);
        Assert.Null(result.PurchaseDate);
        Assert.Null(result.ExpirationDate);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_AlternateLabels_StillExtracts()
    {
        var service = CreateServiceWithText("Item: Headphones  Manufacturer: Sony  Valid Until: 2027-06-30");

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Equal("Headphones", result.ProductName);
        Assert.Equal("Sony", result.Brand);
        Assert.NotNull(result.ExpirationDate);
        Assert.Equal(new DateTime(2027, 6, 30), result.ExpirationDate);
    }

    [Fact]
    public async Task ParseWarrantyImageAsync_InvalidDate_ReturnsNull()
    {
        var service = CreateServiceWithText("Purchase Date: not-a-date  Expiration Date: also-not");

        using var stream = new MemoryStream([1, 2, 3]);
        var result = await service.ParseWarrantyImageAsync(stream, "image/jpeg");

        Assert.Null(result.PurchaseDate);
        Assert.Null(result.ExpirationDate);
    }

    #endregion
}
