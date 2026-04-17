using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;

[ApiController]
[Route("api/[controller]")]
public class SupervisorController : ControllerBase
{
    private readonly AppDbContext _context;

    public SupervisorController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/supervisors
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var supervisors = await _context.Employees
            .Where(e => e.Role == "Supervisor")
            .Include(e => e.ResearchAreas).ThenInclude(sr => sr.ResearchArea)
            .Include(e => e.SupervisedProjects)
            .ToListAsync();

        var resp = supervisors.Select(s => new SupervisorResponse
        {
            Id = s.Id,
            FullName = s.FullName,
            Email = s.Email,
            CreatedAt = s.CreatedAt,
            ResearchAreas = s.ResearchAreas.Select(x => x.ResearchArea.Name).ToList(),
            SupervisedProjectsCount = s.SupervisedProjects.Count
        }).ToList();

        return Ok(resp);
    }

    // GET api/supervisors/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var s = await _context.Employees
            .Include(e => e.ResearchAreas).ThenInclude(sr => sr.ResearchArea)
            .Include(e => e.SupervisedProjects)
            .FirstOrDefaultAsync(e => e.Id == id && e.Role == "Supervisor");

        if (s == null) return NotFound();

        var resp = new SupervisorResponse
        {
            Id = s.Id,
            FullName = s.FullName,
            Email = s.Email,
            CreatedAt = s.CreatedAt,
            ResearchAreas = s.ResearchAreas.Select(x => x.ResearchArea.Name).ToList(),
            SupervisedProjectsCount = s.SupervisedProjects.Count
        };

        return Ok(resp);
    }

    // --- Supervisor admin endpoints (research area assignments) ---

    // GET api/supervisors/research-areas
    [HttpGet("research-areas")]
    public async Task<IActionResult> GetResearchAreas()
    {
        var areas = await _context.ResearchAreas
            .OrderBy(r => r.Name)
            .Select(r => new ResearchAreaDto { Id = r.Id, Name = r.Name })
            .ToListAsync();

        return Ok(areas);
    }

    // POST api/supervisors/assign-research-areas
    [HttpPost("assign-research-areas")]
    public async Task<IActionResult> AssignResearchAreas([FromBody] AssignResearchAreasRequest request)
    {
        var supervisor = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == request.SupervisorId && e.Role == "Supervisor");

        if (supervisor == null)
            return NotFound("Supervisor not found.");

        // Remove existing links for this supervisor
        var existing = _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == request.SupervisorId);

        _context.SupervisorResearchAreas.RemoveRange(existing);

        // Validate research area ids
        var validAreas = await _context.ResearchAreas
            .Where(r => request.ResearchAreaIds.Contains(r.Id))
            .Select(r => r.Id)
            .ToListAsync();

        var newLinks = validAreas.Select(areaId => new Synkademy.Models.SupervisorResearchArea
        {
            SupervisorId = request.SupervisorId,
            ResearchAreaId = areaId
        });

        await _context.SupervisorResearchAreas.AddRangeAsync(newLinks);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Research areas updated successfully", assigned = validAreas });
    }

    // GET api/supervisors/{id}/research-areas
    [HttpGet("{id}/research-areas")]
    public async Task<IActionResult> GetSupervisorResearchAreas(int id)
    {
        var areas = await _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == id)
            .Include(x => x.ResearchArea)
            .Select(x => new ResearchAreaDto { Id = x.ResearchArea.Id, Name = x.ResearchArea.Name })
            .ToListAsync();

        return Ok(areas);
    }
}
