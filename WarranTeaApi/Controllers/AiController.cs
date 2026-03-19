using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarranTeaApi.Services;

namespace WarranTeaApi.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly AiService _aiService;

    public AiController(AiService aiService)
    {
        _aiService = aiService;
    }

    [HttpPost("warranty-query")]
    public async Task<IActionResult> WarrantyQuery([FromBody] WarrantyQueryRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest(new { Message = "A message is required." });

        var history = request.History?.Select(h => new ChatMessage
        {
            Role = h.Role,
            Content = h.Content
        }).ToList();

        var result = await _aiService.AskAsync(request.Message, history);

        return Ok(new WarrantyQueryResponse
        {
            Message = result.Message,
            Success = result.Success
        });
    }
}

public class WarrantyQueryRequest
{
    public string Message { get; set; } = string.Empty;
    public List<ChatHistoryItem>? History { get; set; }
}

public class ChatHistoryItem
{
    public string Role { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

public class WarrantyQueryResponse
{
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; }
}
