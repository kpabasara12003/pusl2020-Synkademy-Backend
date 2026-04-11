using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

[ApiController]
[Route("api/[controller]")]
public class SupervisorController : ControllerBase
{
    private readonly AppDbContext _context;

    public SupervisorController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet("research-areas")]
    public async Task<IActionResult> GetResearchAreas()
    {
        var areas = await _context.ResearchAreas
            .Select(r => new ResearchAreaDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync();

        return Ok(areas);
    }

    [HttpPost("assign-research-areas")]
    public async Task<IActionResult> AssignResearchAreas([FromBody] AssignResearchAreasRequest request)
    {
        var supervisor = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.SupervisorId && e.Role == "Supervisor");

        if (supervisor == null)
            return NotFound("Supervisor not found.");

        var existing = _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == request.SupervisorId);

        _context.SupervisorResearchAreas.RemoveRange(existing);

        var validAreas = await _context.ResearchAreas
            .Where(r => request.ResearchAreaIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();

        //  Insert new selections
        var newLinks = validAreas.Select(areaId => new SupervisorResearchArea
        {
            SupervisorId = request.SupervisorId,
            ResearchAreaId = areaId
        });

        await _context.SupervisorResearchAreas.AddRangeAsync(newLinks);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = "Research areas updated successfully",
            assigned = validAreas
        });
    }

    [HttpGet("{supervisorId}/research-areas")]
    public async Task<IActionResult> GetSupervisorResearchAreas(int supervisorId)
    {
        var areas = await _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == supervisorId)
            .Include(x => x.ResearchArea)
            .Select(x => new ResearchAreaDto
            {
                Id = x.ResearchArea.Id,
                Name = x.ResearchArea.Name
            })
            .ToListAsync();

        return Ok(areas);
    }
}