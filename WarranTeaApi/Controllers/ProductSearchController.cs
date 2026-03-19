using Microsoft.AspNetCore.Mvc;
using WarranTeaApi.Services;

namespace WarranTeaApi.Controllers;

[ApiController]
[Route("api/products/search")]
public class ProductSearchController : ControllerBase
{
    private readonly SearchService _searchService;

    public ProductSearchController(SearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet]
    public async Task<IActionResult> Search(
        [FromQuery] string q,
        [FromQuery] string? brand = null,
        [FromQuery] string? category = null,
        [FromQuery] int top = 10)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { Message = "Query parameter 'q' is required." });

        var results = await _searchService.SearchAsync(q, brand, category, top);
        return Ok(results);
    }

    [HttpPost("image")]
    public async Task<IActionResult> SearchByImage(IFormFile file, [FromQuery] int top = 10)
    {
        if (file == null || file.Length == 0)
            return BadRequest(new { Message = "An image file is required." });

        using var stream = file.OpenReadStream();
        var results = await _searchService.SearchByImageAsync(stream, file.ContentType, top);
        return Ok(results);
    }
}
