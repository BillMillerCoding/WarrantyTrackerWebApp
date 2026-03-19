using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WarranTeaApi.Services;

namespace WarranTeaApi.Tests;

public class AiServiceTests
{
    private const string TestEndpoint = "https://test.openai.azure.com/openai/deployments/gpt/chat/completions?api-version=2024-12-01-preview";
    private const string TestApiKey = "test-api-key";

    private static AiService CreateService(HttpMessageHandler handler)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureOpenAI:Endpoint"] = TestEndpoint,
                ["AzureOpenAI:ApiKey"] = TestApiKey
            })
            .Build();

        var httpClient = new HttpClient(handler);
        var mockFactory = new Mock<IHttpClientFactory>();
        mockFactory.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(httpClient);

        var logger = new Mock<ILogger<AiService>>();

        return new AiService(config, mockFactory.Object, logger.Object);
    }

    private static HttpMessageHandler CreateMockHandler(HttpStatusCode status, string responseBody)
    {
        var mock = new Mock<HttpMessageHandler>();
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = status,
                Content = new StringContent(responseBody)
            });
        return mock.Object;
    }

    [Fact]
    public async Task AskAsync_SuccessfulResponse_ReturnsParsedMessage()
    {
        var responseJson = JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new { message = new { content = "Your warranty covers 2 years." } }
            }
        });

        var service = CreateService(CreateMockHandler(HttpStatusCode.OK, responseJson));

        var result = await service.AskAsync("What does my warranty cover?");

        Assert.True(result.Success);
        Assert.Equal("Your warranty covers 2 years.", result.Message);
    }

    [Fact]
    public async Task AskAsync_WithHistory_IncludesHistoryInRequest()
    {
        var responseJson = JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new { message = new { content = "Follow-up answer" } }
            }
        });

        HttpRequestMessage? capturedRequest = null;
        var mock = new Mock<HttpMessageHandler>();
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .Callback<HttpRequestMessage, CancellationToken>((req, _) => capturedRequest = req)
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(responseJson)
            });

        var service = CreateService(mock.Object);
        var history = new List<ChatMessage>
        {
            new() { Role = "user", Content = "First question" },
            new() { Role = "assistant", Content = "First answer" }
        };

        await service.AskAsync("Follow-up question", history);

        Assert.NotNull(capturedRequest);
        var body = await capturedRequest!.Content!.ReadAsStringAsync();
        Assert.Contains("First question", body);
        Assert.Contains("First answer", body);
        Assert.Contains("Follow-up question", body);
    }

    [Fact]
    public async Task AskAsync_ApiError_ReturnsFailure()
    {
        var service = CreateService(CreateMockHandler(HttpStatusCode.InternalServerError, "Server error"));

        var result = await service.AskAsync("Test question");

        Assert.False(result.Success);
        Assert.Contains("temporarily unavailable", result.Message);
    }

    [Fact]
    public async Task AskAsync_Timeout_ReturnsTimeoutMessage()
    {
        var mock = new Mock<HttpMessageHandler>();
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new TaskCanceledException());

        var service = CreateService(mock.Object);

        var result = await service.AskAsync("Test question");

        Assert.False(result.Success);
        Assert.Contains("timed out", result.Message);
    }

    [Fact]
    public async Task AskAsync_UnexpectedException_ReturnsGenericError()
    {
        var mock = new Mock<HttpMessageHandler>();
        mock.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ThrowsAsync(new InvalidOperationException("Something broke"));

        var service = CreateService(mock.Object);

        var result = await service.AskAsync("Test question");

        Assert.False(result.Success);
        Assert.Contains("unexpected error", result.Message);
    }

    [Fact]
    public void Constructor_MissingEndpoint_Throws()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureOpenAI:ApiKey"] = TestApiKey
            })
            .Build();

        var mockFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<AiService>>();

        Assert.Throws<InvalidOperationException>(() =>
            new AiService(config, mockFactory.Object, logger.Object));
    }

    [Fact]
    public void Constructor_MissingApiKey_Throws()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["AzureOpenAI:Endpoint"] = TestEndpoint
            })
            .Build();

        var mockFactory = new Mock<IHttpClientFactory>();
        var logger = new Mock<ILogger<AiService>>();

        Assert.Throws<InvalidOperationException>(() =>
            new AiService(config, mockFactory.Object, logger.Object));
    }

    [Fact]
    public async Task AskAsync_NullHistory_Succeeds()
    {
        var responseJson = JsonSerializer.Serialize(new
        {
            choices = new[]
            {
                new { message = new { content = "No history response" } }
            }
        });

        var service = CreateService(CreateMockHandler(HttpStatusCode.OK, responseJson));

        var result = await service.AskAsync("Solo question", null);

        Assert.True(result.Success);
        Assert.Equal("No history response", result.Message);
    }
}
