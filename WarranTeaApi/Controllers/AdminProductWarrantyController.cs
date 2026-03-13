using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarranTeaApi.Models.Dto;
using WarranTeaApi.Services;

namespace WarranTeaApi.Controllers;

[ApiController]
[Route("api/admin/product-warranties")]
[Authorize(Roles = "Admin")]
public class AdminProductWarrantyController : ControllerBase
{
    private readonly ProductWarrantyService _service;

    public AdminProductWarrantyController(ProductWarrantyService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAllAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var pw = await _service.GetByIdAsync(id);
        return pw == null ? NotFound() : Ok(pw);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductWarrantyDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var pw = await _service.CreateAsync(dto);
        if (pw == null)
            return BadRequest(new { Message = "Product not found." });

        return CreatedAtAction(nameof(GetById), new { id = pw.Id }, pw);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateProductWarrantyDto dto)
    {
        var pw = await _service.UpdateAsync(id, dto);
        return pw == null ? NotFound() : Ok(pw);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _service.DeleteAsync(id);
        return deleted ? NoContent() : NotFound();
    }

    [HttpPost("reindex")]
    public async Task<IActionResult> ReindexAll()
    {
        var count = await _service.ReindexAllAsync();
        return Ok(new { Message = $"Re-indexed {count} product warranties." });
    }
}
