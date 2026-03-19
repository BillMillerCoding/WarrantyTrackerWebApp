using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarranTeaApi.Services;

namespace WarranTeaApi.Controllers;

[ApiController]
[Route("api/warranties")]
[Authorize]
public class WarrantyController : ControllerBase
{
    private readonly WarrantyService _warrantyService;
    private readonly BlobService _blobService;

    public WarrantyController(WarrantyService warrantyService, BlobService blobService)
    {
        _warrantyService = warrantyService;
        _blobService = blobService;
    }

    private string GetUserId() =>
        User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? throw new UnauthorizedAccessException();

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? query, [FromQuery] string? brand, [FromQuery] string? status)
    {
        var dtos = await _warrantyService.GetAllAsync(GetUserId(), query, brand, status);
        return Ok(dtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var dto = await _warrantyService.GetByIdAsync(GetUserId(), id);
        return dto == null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromForm] Models.Dto.CreateWarrantyDto dto, IFormFile? file)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        using var stream = file?.OpenReadStream();
        var result = await _warrantyService.CreateAsync(GetUserId(), dto, stream, file?.FileName, file?.ContentType);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Models.Dto.UpdateWarrantyDto dto)
    {
        var result = await _warrantyService.UpdateAsync(GetUserId(), id, dto);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _warrantyService.DeleteAsync(GetUserId(), id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("{id}/upload-receipt")]
    public async Task<IActionResult> UploadReceipt(int id, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var result = await _warrantyService.UploadReceiptAsync(GetUserId(), id, stream, file.FileName, file.ContentType);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpGet("{id}/receipt")]
    public async Task<IActionResult> GetReceipt(int id)
    {
        var dto = await _warrantyService.GetByIdAsync(GetUserId(), id);
        if (dto?.ReceiptUrl == null) return NotFound();
        var stream = await _blobService.DownloadAsync(dto.ReceiptUrl);
        var ext = Path.GetExtension(new Uri(dto.ReceiptUrl).AbsolutePath).ToLowerInvariant();
        var contentType = ext switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            _ => "application/octet-stream"
        };
        return File(stream, contentType);
    }

    [HttpGet("{id}/extracted-text")]
    public async Task<IActionResult> GetExtractedText(int id)
    {
        var dto = await _warrantyService.GetByIdAsync(GetUserId(), id);
        if (dto?.WarrantyDocUrl == null) return NotFound();
        var stream = await _blobService.DownloadAsync(dto.WarrantyDocUrl);
        return File(stream, "text/plain");
    }

    [HttpPost("{id}/upload-doc")]
    public async Task<IActionResult> UploadWarrantyDoc(int id, IFormFile file)
    {
        using var stream = file.OpenReadStream();
        var result = await _warrantyService.UploadWarrantyDocAsync(GetUserId(), id, stream, file.FileName, file.ContentType);
        return result == null ? NotFound() : Ok(result);
    }
}
