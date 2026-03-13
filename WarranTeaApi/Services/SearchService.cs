using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Azure.Search.Documents.Models;
using WarranTeaApi.Models;
using WarranTeaApi.Models.Dto;

namespace WarranTeaApi.Services;

public class SearchService
{
    private readonly SearchIndexClient _indexClient;
    private readonly SearchClient _searchClient;
    private readonly EmbeddingService _embeddingService;
    private readonly string _indexName;

    public SearchService(IConfiguration configuration, EmbeddingService embeddingService)
    {
        var endpointStr = configuration["AzureAISearch:Endpoint"]
            ?? throw new InvalidOperationException("Missing config: AzureAISearch:Endpoint");
        var apiKeyStr = configuration["AzureAISearch:AdminApiKey"]
            ?? throw new InvalidOperationException("Missing config: AzureAISearch:AdminApiKey");
        _indexName = configuration["AzureAISearch:IndexName"]
            ?? throw new InvalidOperationException("Missing config: AzureAISearch:IndexName");

        var endpoint = new Uri(endpointStr);
        var apiKey = new AzureKeyCredential(apiKeyStr);
        _indexClient = new SearchIndexClient(endpoint, apiKey);
        _searchClient = new SearchClient(endpoint, _indexName, apiKey);
        _embeddingService = embeddingService;
    }

    /// <summary>
    /// Create the search index only if it does not already exist.
    /// </summary>
    public async Task CreateIndexIfNotExistsAsync()
    {
        try
        {
            await _indexClient.GetIndexAsync(_indexName);
            return; // Index already exists, nothing to do
        }
        catch (Azure.RequestFailedException ex) when (ex.Status == 404)
        {
            // Index doesn't exist, create it below
        }

        var vectorSearch = new VectorSearch();
        vectorSearch.Profiles.Add(new VectorSearchProfile("vector-profile", "hnsw-config")
        {
            VectorizerName = null
        });
        vectorSearch.Algorithms.Add(new HnswAlgorithmConfiguration("hnsw-config")
        {
            Parameters = new HnswParameters
            {
                Metric = VectorSearchAlgorithmMetric.Cosine,
                M = 4,
                EfConstruction = 400,
                EfSearch = 500
            }
        });

        var semanticConfig = new SemanticConfiguration("semantic-config", new SemanticPrioritizedFields
        {
            TitleField = new SemanticField("productName"),
            ContentFields = { new SemanticField("description") },
            KeywordsFields =
            {
                new SemanticField("brand"),
                new SemanticField("category"),
                new SemanticField("coverageType"),
                new SemanticField("provider")
            }
        });

        var semanticSearch = new SemanticSearch();
        semanticSearch.Configurations.Add(semanticConfig);
        semanticSearch.DefaultConfigurationName = "semantic-config";

        var index = new SearchIndex(_indexName)
        {
            VectorSearch = vectorSearch,
            SemanticSearch = semanticSearch,
            Fields =
            {
                new SimpleField("id", SearchFieldDataType.String) { IsKey = true, IsFilterable = true },
                new SimpleField("productWarrantyId", SearchFieldDataType.Int32) { IsFilterable = true },
                new SimpleField("productId", SearchFieldDataType.Int32) { IsFilterable = true },
                new SearchableField("productName") { IsFilterable = true, IsSortable = true },
                new SearchableField("brand") { IsFilterable = true, IsFacetable = true },
                new SearchableField("category") { IsFilterable = true, IsFacetable = true },
                new SearchableField("coverageType") { IsFilterable = true, IsFacetable = true },
                new SimpleField("durationMonths", SearchFieldDataType.Int32) { IsFilterable = true, IsSortable = true },
                new SearchableField("provider") { IsFilterable = true, IsFacetable = true },
                new SearchableField("description"),
                new SimpleField("url", SearchFieldDataType.String),
                new SimpleField("imageUrl", SearchFieldDataType.String),
                new SearchField("contentVector", SearchFieldDataType.Collection(SearchFieldDataType.Single))
                {
                    IsSearchable = true,
                    VectorSearchDimensions = 1024,
                    VectorSearchProfileName = "vector-profile"
                }
            }
        };

        await _indexClient.CreateIndexAsync(index);
    }

    /// <summary>
    /// Index a single ProductWarranty document (with its parent Product data) into the search index.
    /// </summary>
    public async Task IndexDocumentAsync(ProductWarranty pw)
    {
        var textToEmbed = $"{pw.Product.Name} {pw.Product.Brand} {pw.Product.Category} {pw.CoverageType} {pw.Provider} {pw.Description}";
        var vector = await _embeddingService.EmbedTextAsync(textToEmbed);

        var doc = new SearchDocument
        {
            ["id"] = pw.Id.ToString(),
            ["productWarrantyId"] = pw.Id,
            ["productId"] = pw.ProductId,
            ["productName"] = pw.Product.Name,
            ["brand"] = pw.Product.Brand,
            ["category"] = pw.Product.Category,
            ["coverageType"] = pw.CoverageType,
            ["durationMonths"] = pw.DurationMonths,
            ["provider"] = pw.Provider,
            ["description"] = pw.Description ?? string.Empty,
            ["url"] = pw.Url ?? string.Empty,
            ["imageUrl"] = pw.Product.ImageUrl ?? string.Empty,
            ["contentVector"] = vector
        };

        var batch = IndexDocumentsBatch.MergeOrUpload(new[] { doc });
        await _searchClient.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Index multiple ProductWarranty documents in a single batch.
    /// </summary>
    public async Task IndexDocumentsAsync(IEnumerable<ProductWarranty> warranties)
    {
        var docs = new List<SearchDocument>();

        foreach (var pw in warranties)
        {
            var textToEmbed = $"{pw.Product.Name} {pw.Product.Brand} {pw.Product.Category} {pw.CoverageType} {pw.Provider} {pw.Description}";
            var vector = await _embeddingService.EmbedTextAsync(textToEmbed);

            docs.Add(new SearchDocument
            {
                ["id"] = pw.Id.ToString(),
                ["productWarrantyId"] = pw.Id,
                ["productId"] = pw.ProductId,
                ["productName"] = pw.Product.Name,
                ["brand"] = pw.Product.Brand,
                ["category"] = pw.Product.Category,
                ["coverageType"] = pw.CoverageType,
                ["durationMonths"] = pw.DurationMonths,
                ["provider"] = pw.Provider,
                ["description"] = pw.Description ?? string.Empty,
                ["url"] = pw.Url ?? string.Empty,
                ["imageUrl"] = pw.Product.ImageUrl ?? string.Empty,
                ["contentVector"] = vector
            });
        }

        var batch = IndexDocumentsBatch.MergeOrUpload(docs);
        await _searchClient.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Remove a document from the search index by ProductWarranty ID.
    /// </summary>
    public async Task DeleteDocumentAsync(int productWarrantyId)
    {
        var batch = IndexDocumentsBatch.Delete("id", new[] { productWarrantyId.ToString() });
        await _searchClient.IndexDocumentsAsync(batch);
    }

    /// <summary>
    /// Perform a hybrid search (text + vector) with semantic ranking.
    /// </summary>
    public async Task<ProductWarrantySearchResponseDto> SearchAsync(string query, string? brandFilter = null, string? categoryFilter = null, int top = 10)
    {
        var queryVector = await _embeddingService.EmbedTextAsync(query);

        var options = new SearchOptions
        {
            Size = top,
            QueryType = SearchQueryType.Semantic,
            SemanticSearch = new SemanticSearchOptions
            {
                SemanticConfigurationName = "semantic-config"
            },
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(queryVector)
                    {
                        KNearestNeighborsCount = top,
                        Fields = { "contentVector" }
                    }
                }
            },
            Select =
            {
                "id", "productWarrantyId", "productId", "productName", "brand",
                "category", "coverageType", "durationMonths", "provider",
                "description", "url", "imageUrl"
            }
        };

        // Build OData filter
        var filters = new List<string>();
        if (!string.IsNullOrEmpty(brandFilter))
            filters.Add($"brand eq '{brandFilter.Replace("'", "''")}'");
        if (!string.IsNullOrEmpty(categoryFilter))
            filters.Add($"category eq '{categoryFilter.Replace("'", "''")}'");
        if (filters.Count > 0)
            options.Filter = string.Join(" and ", filters);

        var response = await _searchClient.SearchAsync<SearchDocument>(query, options);

        var results = new List<ProductWarrantySearchResultDto>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            results.Add(MapToDto(result));
        }

        return new ProductWarrantySearchResponseDto
        {
            Results = results,
            TotalCount = results.Count
        };
    }

    /// <summary>
    /// Perform a vector-only search using an image embedding.
    /// </summary>
    public async Task<ProductWarrantySearchResponseDto> SearchByImageAsync(Stream imageStream, string contentType, int top = 10)
    {
        var imageVector = await _embeddingService.EmbedImageAsync(imageStream, contentType);

        var options = new SearchOptions
        {
            Size = top,
            VectorSearch = new VectorSearchOptions
            {
                Queries =
                {
                    new VectorizedQuery(imageVector)
                    {
                        KNearestNeighborsCount = top,
                        Fields = { "contentVector" }
                    }
                }
            },
            Select =
            {
                "id", "productWarrantyId", "productId", "productName", "brand",
                "category", "coverageType", "durationMonths", "provider",
                "description", "url", "imageUrl"
            }
        };

        var response = await _searchClient.SearchAsync<SearchDocument>(null, options);

        var results = new List<ProductWarrantySearchResultDto>();
        await foreach (var result in response.Value.GetResultsAsync())
        {
            results.Add(MapToDto(result));
        }

        return new ProductWarrantySearchResponseDto
        {
            Results = results,
            TotalCount = results.Count
        };
    }

    private static ProductWarrantySearchResultDto MapToDto(SearchResult<SearchDocument> result)
    {
        var doc = result.Document;
        return new ProductWarrantySearchResultDto
        {
            ProductWarrantyId = doc.TryGetValue("productWarrantyId", out var pwId) ? Convert.ToInt32(pwId) : 0,
            ProductId = doc.TryGetValue("productId", out var pId) ? Convert.ToInt32(pId) : 0,
            ProductName = doc.TryGetValue("productName", out var name) ? name?.ToString() ?? string.Empty : string.Empty,
            Brand = doc.TryGetValue("brand", out var brand) ? brand?.ToString() ?? string.Empty : string.Empty,
            Category = doc.TryGetValue("category", out var cat) ? cat?.ToString() ?? string.Empty : string.Empty,
            CoverageType = doc.TryGetValue("coverageType", out var cov) ? cov?.ToString() ?? string.Empty : string.Empty,
            DurationMonths = doc.TryGetValue("durationMonths", out var dur) ? Convert.ToInt32(dur) : 0,
            Provider = doc.TryGetValue("provider", out var prov) ? prov?.ToString() ?? string.Empty : string.Empty,
            Description = doc.TryGetValue("description", out var desc) ? desc?.ToString() : null,
            Url = doc.TryGetValue("url", out var url) ? url?.ToString() : null,
            ImageUrl = doc.TryGetValue("imageUrl", out var img) ? img?.ToString() : null,
            Score = result.Score
        };
    }
}
