using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

namespace Synkademy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BlindReviewController : ControllerBase
{
    private readonly AppDbContext _context;

    public BlindReviewController(AppDbContext context)
    {
        _context = context;
    }

    // GET MATCHING PROJECTS
    [HttpGet("{supervisorId}/projects")]
    public async Task<IActionResult> GetProjects(int supervisorId)
    {
        if (supervisorId <= 0)
            return BadRequest("Invalid supervisor ID");

        // Get supervisor research areas
        var supervisorAreaIds = await _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == supervisorId)
            .Select(x => x.ResearchAreaId)
            .ToListAsync();

        var projects = await _context.Projects
            .Where(p => p.Status == "Pending" && p.SupervisorId == null)
            // Match supervisor expertise
            .Where(p => p.ProjectResearchAreas
                .Any(pr => supervisorAreaIds.Contains(pr.ResearchAreaId)))
            //Exclude already interested projects
            .Where(p => !p.Interests
                .Any(i => i.SupervisorId == supervisorId))
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.ShortDescription,
                p.TechStack,
                ResearchAreas = p.ProjectResearchAreas
                    .Select(x => x.ResearchArea.Name)
                    .ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    // PROJECT DETAILS (BLIND)
    [HttpGet("project/{projectId}")]
    public async Task<IActionResult> GetProjectDetails(int projectId)
    {
        var project = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.ShortDescription,
                p.Abstract,
                p.TechStack,
                p.CreatedAt,

                ResearchAreas = p.ProjectResearchAreas
                    .Select(x => x.ResearchArea.Name)
                    .ToList(),

                Tags = p.Tags
                    .Select(t => t.Tag.Name)
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (project == null)
            return NotFound("Project not found.");

        return Ok(project);
    }

    //ADD INTEREST
    [HttpPost("{supervisorId}/interest/{projectId}")]
    public async Task<IActionResult> AddInterest(int supervisorId, int projectId)
    {
  
        var projectExists = await _context.Projects
            .AnyAsync(p => p.Id == projectId && p.Status == "Pending");

        if (!projectExists)
            return BadRequest("Project not available.");

        var exists = await _context.ProjectInterests
            .AnyAsync(x => x.SupervisorId == supervisorId && x.ProjectId == projectId);

        if (exists)
            return Conflict("Already interested.");

        _context.ProjectInterests.Add(new ProjectInterest
        {
            SupervisorId = supervisorId,
            ProjectId = projectId
        });

        await _context.SaveChangesAsync();

        return Ok(new { message = "Interest added." });
    }

    // REMOVE INTEREST
    [HttpDelete("{supervisorId}/interest/{projectId}")]
    public async Task<IActionResult> RemoveInterest(int supervisorId, int projectId)
    {
        var interest = await _context.ProjectInterests
            .FirstOrDefaultAsync(x => x.SupervisorId == supervisorId && x.ProjectId == projectId);

        if (interest == null)
            return NotFound("Interest not found.");

        _context.ProjectInterests.Remove(interest);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Interest removed." });
    }

    // ASSIGN PROJECT
    [HttpPost("{supervisorId}/assign/{projectId}")]
    public async Task<IActionResult> AssignProject(int supervisorId, int projectId)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var project = await _context.Projects
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null)
                return NotFound("Project not found.");

            if (project.SupervisorId != null)
                return BadRequest("Project already assigned.");

            if (project.Status != "Pending")
                return BadRequest("Project is not available.");

            // Assign supervisor
            project.SupervisorId = supervisorId;
            project.Status = "Matched";

            //DELETE ALL INTERESTS 
            var interests = await _context.ProjectInterests
                .Where(x => x.ProjectId == projectId)
                .ToListAsync();

            _context.ProjectInterests.RemoveRange(interests);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return Ok(new
            {
                message = "Project assigned successfully. All interests cleared."
            });
        }
        catch
        {
            await transaction.RollbackAsync();
            return StatusCode(500, "Assignment failed.");
        }
    }

    //Get Interested Projects
    [HttpGet("{supervisorId}/interests")]
    public async Task<IActionResult> GetInterestedProjects(int supervisorId)
    {
        var projects = await _context.Projects
            .Where(p => p.Status == "Pending" && p.SupervisorId == null)
            .Where(p => p.Interests.Any(i => i.SupervisorId == supervisorId))
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.ShortDescription,
                p.TechStack,
                ResearchAreas = p.ProjectResearchAreas
                    .Select(x => x.ResearchArea.Name)
                    .ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    //Get Matched Projects (Banners)
    [HttpGet("{supervisorId}/assigned")]
    public async Task<IActionResult> GetAssignedProjects(
    int supervisorId,
    [FromQuery] string? search)
    {
        if (supervisorId <= 0)
            return BadRequest("Invalid supervisor ID");

        var query = _context.Projects
            .Where(p => p.SupervisorId == supervisorId);

        //  SEARCH (SQL LIKE)
        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";

            query = query.Where(p =>
                EF.Functions.Like(p.Title, pattern) ||
                EF.Functions.Like(p.TechStack!, pattern) ||
                EF.Functions.Like(p.ShortDescription!, pattern)
            );
        }

        var projects = await query
            //  ORDER BY LATEST
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new
            {
                p.Id,
                p.Title,

                Student = new
                {
                    p.Student.FullName,
                    p.Student.StudentNumber
                },

                p.ShortDescription,
                p.TechStack,

                ResearchAreas = p.ProjectResearchAreas
                    .Select(x => x.ResearchArea.Name)
                    .ToList()
            })
            .ToListAsync();

        return Ok(projects);
    }

    //Get Matched Projects (In-depth)
    [HttpGet("assigned/{projectId}/details")]
    public async Task<IActionResult> GetAssignedProjectDetails(int projectId)
    {
        var project = await _context.Projects
            .Where(p => p.Id == projectId)
            .Select(p => new
            {
                p.Id,
                p.Title,
                p.ShortDescription,
                p.Abstract,
                p.TechStack,
                p.CreatedAt,
                p.Status,
                
                Student = new
                {
                    p.Student.FullName,
                    p.Student.StudentNumber,
                    p.Student.Email
                },
                ResearchAreas = p.ProjectResearchAreas
                    .Select(x => x.ResearchArea.Name)
                    .ToList(),
                Tags = p.Tags
            })
            .FirstOrDefaultAsync();

        if (project == null) return NotFound("Project details not found.");

        return Ok(project);
    }

}