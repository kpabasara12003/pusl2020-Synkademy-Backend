using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

[ApiController]
[Route("api/[controller]")]
public class ProjectResearchAreasController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectResearchAreasController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/projectresearchareas/project/{projectId}
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var project = await _context.Projects
            .Include(p => p.ProjectResearchAreas).ThenInclude(pr => pr.ResearchArea)
            .FirstOrDefaultAsync(p => p.Id == projectId);

        if (project == null) return NotFound();

        var list = project.ProjectResearchAreas.Select(pr => new { pr.ResearchAreaId, pr.ResearchArea.Name }).ToList();
        return Ok(list);
    }

    // POST api/projectresearchareas
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProjectResearchAreaRequest request)
    {
        var project = await _context.Projects.FindAsync(request.ProjectId);
        if (project == null) return NotFound("Project not found.");

        var ra = await _context.ResearchAreas.FindAsync(request.ResearchAreaId);
        if (ra == null) return NotFound("Research area not found.");

        var exists = await _context.Set<ProjectResearchArea>().AnyAsync(x => x.ProjectId == request.ProjectId && x.ResearchAreaId == request.ResearchAreaId);
        if (exists) return Conflict("Link already exists.");

        var link = new ProjectResearchArea { ProjectId = request.ProjectId, ResearchAreaId = request.ResearchAreaId };
        _context.Set<ProjectResearchArea>().Add(link);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Link created." });
    }
}
