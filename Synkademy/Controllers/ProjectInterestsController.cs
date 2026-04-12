using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectInterestsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectInterestsController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/projectinterests/project/{projectId}
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var list = await _context.ProjectInterests
            .Where(pi => pi.ProjectId == projectId)
            .Include(pi => pi.Supervisor)
            .Select(pi => new {
                pi.Id,
                pi.ProjectId,
                pi.SupervisorId,
                SupervisorName = pi.Supervisor != null ? pi.Supervisor.FullName : null,
                pi.Status,
                pi.CreatedAt
            })
            .ToListAsync();

        return Ok(list);
    }

    // POST api/projectinterests
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectInterestRequest request)
    {
        var project = await _context.Projects.FindAsync(request.ProjectId);
        if (project == null) return NotFound("Project not found.");

        var supervisor = await _context.Employees.FindAsync(request.SupervisorId);
        if (supervisor == null) return NotFound("Supervisor not found.");

        var interest = new ProjectInterest
        {
            ProjectId = request.ProjectId,
            SupervisorId = request.SupervisorId,
            Status = string.IsNullOrWhiteSpace(request.Status) ? "Pending" : request.Status,
            CreatedAt = DateTime.UtcNow
        };

        _context.ProjectInterests.Add(interest);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Interest recorded.", id = interest.Id });
    }
}
