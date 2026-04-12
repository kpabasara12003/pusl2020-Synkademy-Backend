using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

[ApiController]
[Route("api/[controller]")]
public class ResearchAreasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ResearchAreasController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/researchareas
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.ResearchAreas
            .OrderBy(r => r.Name)
            .Select(r => new { r.Id, r.Name })
            .ToListAsync();

        return Ok(list);
    }

    // POST api/researchareas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateResearchAreaRequest request)
    {
        if (string.IsNullOrWhiteSpace(request?.Name))
            return BadRequest("Name is required.");

        var exists = await _context.ResearchAreas.AnyAsync(r => r.Name == request.Name);
        if (exists) return Conflict("Research area already exists.");

        var ra = new ResearchArea { Name = request.Name };
        _context.ResearchAreas.Add(ra);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Research area created.", id = ra.Id });
    }
}
