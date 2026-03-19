using System.Text;
using System.Text.Json;

namespace WarranTeaApi.Services;

public class AiService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _endpoint;
    private readonly string _apiKey;
    private readonly ILogger<AiService> _logger;

    private const string SystemPrompt = """
        You are a warranty assistant for the WarranTea application.
        You ONLY answer warranty-related questions. If a user asks something unrelated,
        respond with: "I can only assist with warranty-related questions."

        Your capabilities:
        1. Extract warranty details from OCR text or user descriptions
        2. Answer questions about warranty status, coverage, and terms
        3. Summarize receipts and warranty documents
        4. Identify missing warranty fields

        When extracting warranty data from text, return a JSON block in this format:
        ```json
        {
          "productName": "",
          "brand": "",
          "category": "",
          "coverageType": "",
          "purchaseDate": "",
          "expirationDate": "",
          "confidence": 0.0,
          "notes": ""
        }
        ```
        Dates should be in YYYY-MM-DD format. Confidence is 0.0 to 1.0.

        For general warranty questions, respond in clear, concise natural language.
        Always be helpful and accurate. If you are unsure, say so.
        """;

    public AiService(IConfiguration configuration, IHttpClientFactory httpClientFactory, ILogger<AiService> logger)
    {
        _endpoint = configuration["AzureOpenAI:Endpoint"]
            ?? throw new InvalidOperationException("Missing config: AzureOpenAI:Endpoint");
        _apiKey = configuration["AzureOpenAI:ApiKey"]
            ?? throw new InvalidOperationException("Missing config: AzureOpenAI:ApiKey");
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<AiResponse> AskAsync(string userMessage, List<ChatMessage>? history = null)
    {
        var messages = new List<object>
        {
            new { role = "system", content = SystemPrompt }
        };

        if (history != null)
        {
            foreach (var msg in history)
            {
                messages.Add(new { role = msg.Role, content = msg.Content });
            }
        }

        messages.Add(new { role = "user", content = userMessage });

        var payload = new
        {
            messages,
            max_completion_tokens = 1024
        };

        var json = JsonSerializer.Serialize(payload);
        using var client = _httpClientFactory.CreateClient();
        client.DefaultRequestHeaders.Add("api-key", _apiKey);
        client.Timeout = TimeSpan.FromSeconds(30);

        try
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(_endpoint, content);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                _logger.LogError("Azure OpenAI returned {Status}: {Body}", response.StatusCode, errorBody);
                return new AiResponse
                {
                    Message = "Sorry, the AI service is temporarily unavailable. Please try again.",
                    Success = false
                };
            }

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var assistantMessage = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString() ?? string.Empty;

            return new AiResponse
            {
                Message = assistantMessage,
                Success = true
            };
        }
        catch (TaskCanceledException)
        {
            _logger.LogWarning("Azure OpenAI request timed out");
            return new AiResponse
            {
                Message = "The request timed out. Please try again.",
                Success = false
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling Azure OpenAI");
            return new AiResponse
            {
                Message = "An unexpected error occurred. Please try again.",
                Success = false
            };
        }
    }
}

public class ChatMessage
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class AiResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}
