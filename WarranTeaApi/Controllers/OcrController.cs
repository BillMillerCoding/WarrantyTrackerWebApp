using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarranTeaApi.Services;

namespace WarranTeaApi.Controllers;

[ApiController]
[Route("api/ocr")]
[Authorize]
public class OcrController : ControllerBase
{
    private readonly OcrService _ocrService;

    public OcrController(OcrService ocrService)
    {
        _ocrService = ocrService;
    }

    [HttpPost("parse-warranty")]
    public async Task<IActionResult> ParseWarrantyFromImage(IFormFile file)
    {
        if (file.Length == 0)
            return BadRequest(new { Message = "No file provided." });

        using var stream = file.OpenReadStream();
        var result = await _ocrService.ParseWarrantyImageAsync(stream, file.ContentType);

        return Ok(result);
    }
}
