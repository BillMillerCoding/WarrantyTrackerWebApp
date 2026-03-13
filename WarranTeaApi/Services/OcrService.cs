using System.Globalization;
using System.Text.RegularExpressions;

namespace WarranTeaApi.Services;

public partial class OcrService
{
    private readonly EmbeddingService _embeddingService;

    public OcrService(EmbeddingService embeddingService)
    {
        _embeddingService = embeddingService;
    }

    public async Task<ParsedWarrantyResult> ParseWarrantyImageAsync(Stream imageStream, string contentType)
    {
        var extractedText = await _embeddingService.ExtractTextAsync(imageStream, contentType);

        if (string.IsNullOrWhiteSpace(extractedText))
            return new ParsedWarrantyResult { RawText = string.Empty };

        return ParseWarrantyFields(extractedText);
    }

    private static ParsedWarrantyResult ParseWarrantyFields(string text)
    {
        var lines = text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var allText = string.Join(" ", lines);

        return new ParsedWarrantyResult
        {
            RawText = text,
            ProductName = ExtractField(allText, ProductNamePattern()),
            Brand = ExtractField(allText, BrandPattern()),
            Category = ExtractField(allText, CategoryPattern()),
            CoverageType = ExtractField(allText, CoveragePattern()),
            PurchaseDate = ExtractDate(allText, PurchaseDatePattern()),
            ExpirationDate = ExtractDate(allText, ExpirationDatePattern()),
            Notes = null
        };
    }

    private static string? ExtractField(string text, Regex pattern)
    {
        var match = pattern.Match(text);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private static DateTime? ExtractDate(string text, Regex pattern)
    {
        var match = pattern.Match(text);
        if (!match.Success) return null;

        var dateStr = match.Groups[1].Value.Trim();
        return DateTime.TryParse(dateStr, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : null;
    }

    [GeneratedRegex(@"(?:product\s*name|item|product)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ProductNamePattern();

    [GeneratedRegex(@"(?:brand|manufacturer|made by|mfg)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex BrandPattern();

    [GeneratedRegex(@"(?:category|type|department)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex CategoryPattern();

    [GeneratedRegex(@"(?:coverage|warranty\s*type|plan|protection)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex CoveragePattern();

    [GeneratedRegex(@"(?:purchase\s*date|date\s*of\s*purchase|bought|purchased)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex PurchaseDatePattern();

    [GeneratedRegex(@"(?:expir\w*\s*date|valid\s*(?:until|through|thru)|end\s*date|expires?)\s*[:\-]\s*(.+?)(?:\s{2,}|$)", RegexOptions.IgnoreCase)]
    private static partial Regex ExpirationDatePattern();
}

public class ParsedWarrantyResult
{
    public string RawText { get; set; } = string.Empty;
    public string? ProductName { get; set; }
    public string? Brand { get; set; }
    public string? Category { get; set; }
    public string? CoverageType { get; set; }
    public DateTime? PurchaseDate { get; set; }
    public DateTime? ExpirationDate { get; set; }
    public string? Notes { get; set; }
}
