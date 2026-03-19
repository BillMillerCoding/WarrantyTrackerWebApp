using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace WarranTeaApi.Services;

public class EmbeddingService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _endpoint;
    private readonly string _apiKey;
    private const string ApiVersion = "2024-02-01";

    public EmbeddingService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _endpoint = configuration["AzureAIVision:Endpoint"]
            ?? throw new InvalidOperationException("Missing config: AzureAIVision:Endpoint");
        _endpoint = _endpoint.TrimEnd('/');
        _apiKey = configuration["AzureAIVision:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: AzureAIVision:ApiKey");
        _httpClientFactory = httpClientFactory;
    }

    private HttpClient CreateClient()
    {
        var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _apiKey);
        return client;
    }

    private static async Task<HttpResponseMessage> SendWithRetryAsync(HttpClient client, Func<HttpRequestMessage> requestFactory, int maxRetries = 5)
    {
        for (int attempt = 0; ; attempt++)
        {
            using var request = requestFactory();
            var response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests && attempt < maxRetries)
            {
                var retryAfter = response.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(Math.Pow(2, attempt + 1));
                await Task.Delay(retryAfter);
                continue;
            }

            response.EnsureSuccessStatusCode();
            return response;
        }
    }

    /// <summary>
    /// Generate a vector embedding from text using Azure AI Vision multimodal embeddings.
    /// </summary>
    public virtual async Task<float[]> EmbedTextAsync(string text)
    {
        var url = $"{_endpoint}/computervision/retrieval:vectorizeText?api-version={ApiVersion}&model-version=2023-04-15";
        var payload = JsonSerializer.Serialize(new { text });

        using var client = CreateClient();
        using var response = await SendWithRetryAsync(client, () =>
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            return req;
        });

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<VectorResponse>(json);

        return result?.Vector ?? throw new InvalidOperationException("Failed to get text embedding.");
    }

    /// <summary>
    /// Generate a vector embedding from an image stream using Azure AI Vision multimodal embeddings.
    /// </summary>
    public virtual async Task<float[]> EmbedImageAsync(Stream imageStream, string contentType = "application/octet-stream")
    {
        var url = $"{_endpoint}/computervision/retrieval:vectorizeImage?api-version={ApiVersion}&model-version=2023-04-15";
        var imageBytes = await ReadStreamBytesAsync(imageStream);

        using var client = CreateClient();
        using var response = await SendWithRetryAsync(client, () =>
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            var c = new ByteArrayContent(imageBytes);
            c.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            req.Content = c;
            return req;
        });

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<VectorResponse>(json);

        return result?.Vector ?? throw new InvalidOperationException("Failed to get image embedding.");
    }

    /// <summary>
    /// Generate a vector embedding from an image at a public URL using Azure AI Vision multimodal embeddings.
    /// </summary>
    public virtual async Task<float[]> EmbedImageUrlAsync(string imageUrl)
    {
        var url = $"{_endpoint}/computervision/retrieval:vectorizeImage?api-version={ApiVersion}&model-version=2023-04-15";
        var payload = JsonSerializer.Serialize(new { url = imageUrl });

        using var client = CreateClient();
        using var response = await SendWithRetryAsync(client, () =>
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            req.Content = new StringContent(payload, Encoding.UTF8, "application/json");
            return req;
        });

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<VectorResponse>(json);

        return result?.Vector ?? throw new InvalidOperationException("Failed to get image embedding from URL.");
    }

    /// <summary>
    /// Extract text from an image using Azure AI Vision Read (OCR) API.
    /// </summary>
    public virtual async Task<string> ExtractTextAsync(Stream imageStream, string contentType = "application/octet-stream")
    {
        var url = $"{_endpoint}/computervision/imageanalysis:analyze?api-version=2024-02-01&features=read";
        var imageBytes = await ReadStreamBytesAsync(imageStream);

        using var client = CreateClient();
        using var response = await SendWithRetryAsync(client, () =>
        {
            var req = new HttpRequestMessage(HttpMethod.Post, url);
            var c = new ByteArrayContent(imageBytes);
            c.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            req.Content = c;
            return req;
        });

        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);

        var lines = new List<string>();
        if (doc.RootElement.TryGetProperty("readResult", out var readResult) &&
            readResult.TryGetProperty("blocks", out var blocks))
        {
            foreach (var block in blocks.EnumerateArray())
            {
                if (block.TryGetProperty("lines", out var blockLines))
                {
                    foreach (var line in blockLines.EnumerateArray())
                    {
                        if (line.TryGetProperty("text", out var text))
                            lines.Add(text.GetString() ?? string.Empty);
                    }
                }
            }
        }

        return string.Join("\n", lines);
    }

    private class VectorResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("vector")]
        public float[]? Vector { get; set; }
    }

    private static async Task<byte[]> ReadStreamBytesAsync(Stream stream)
    {
        using var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        return ms.ToArray();
    }
}
