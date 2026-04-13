using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using System.Linq;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private readonly AppDbContext _context;

    public ProjectsController(AppDbContext context)
    {
        _context = context;
    }

    // Create a new project proposal
    [HttpPost("create")]
    public async Task<IActionResult> CreateProject([FromBody] CreateProjectRequest request)
    {
        var student = await _context.Students.FindAsync(request.StudentId);
        if (student == null) return NotFound("Student not found.");

        var hasActive = await _context.Projects.AnyAsync(p => p.StudentId == request.StudentId && p.Status != "Withdrawn" && p.Status != "Rejected");
        if (hasActive) return BadRequest("Only one active proposal allowed per student.");

        var project = new Project
        {
            Title = request.Title,
            ShortDescription = request.ShortDescription,
            Abstract = request.Abstract,
            TechStack = request.TechStack,
            ProposalFilePath = request.ProposalFilePath,
            StudentId = request.StudentId,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };

        // Attach research areas and tags by id
        if (request.ResearchAreas != null)
        {
            foreach (var raId in request.ResearchAreas.Distinct())
            {
                var ra = await _context.ResearchAreas.FindAsync(raId);
                if (ra == null)
                {
                    return NotFound($"Research area with id {raId} not found.");
                }
                project.ProjectResearchAreas.Add(new ProjectResearchArea { Project = project, ResearchArea = ra });
            }
        }

        if (request.Tags != null)
        {
            foreach (var tagId in request.Tags.Distinct())
            {
                var tag = await _context.Tags.FindAsync(tagId);
                if (tag == null)
                {
                    return NotFound($"Tag with id {tagId} not found.");
                }
                project.Tags.Add(new ProjectTag { Project = project, Tag = tag, TagId = tag.Id });
            }
        }

        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Project proposal created.", projectId = project.Id });
    }

    // Get all project details for a student (supervisor details revealed only when matched)
    [HttpGet("student/{studentId}")]
    public async Task<IActionResult> GetStudentProjects(int studentId)
    {
        var list = await _context.Projects
            .Where(p => p.StudentId == studentId)
            .Include(p => p.Supervisor)
            .Include(p => p.ProjectResearchAreas).ThenInclude(pr => pr.ResearchArea)
            .Include(p => p.Tags).ThenInclude(pt => pt.Tag)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        var resp = list.Select(project => new ProjectResponse
        {
            Id = project.Id,
            Title = project.Title,
            ShortDescription = project.ShortDescription,
            Abstract = project.Abstract,
            TechStack = project.TechStack,
            StudentId = project.StudentId,
            SupervisorId = project.SupervisorId,
            Status = project.Status,
            CreatedAt = project.CreatedAt,
            ProposalFilePath = project.ProposalFilePath,
            ResearchAreas = project.ProjectResearchAreas.Select(x => x.ResearchArea.Name).ToList(),
            Tags = project.Tags.Select(x => x.Tag.Name).ToList(),
            SupervisorName = project.Status == "Matched" && project.Supervisor != null ? project.Supervisor.FullName : null,
            SupervisorEmail = project.Status == "Matched" && project.Supervisor != null ? project.Supervisor.Email : null
        }).ToList();

        return Ok(resp);
    }
}
