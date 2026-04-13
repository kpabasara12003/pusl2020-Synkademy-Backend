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
        var supervisorAreaIds = await _context.SupervisorResearchAreas
            .Where(x => x.SupervisorId == supervisorId)
            .Select(x => x.ResearchAreaId)
            .ToListAsync();

        var projects = await _context.Projects
            .Where(p => p.Status == "Pending" && p.SupervisorId == null)
            .Where(p => p.ProjectResearchAreas
                .Any(pr => supervisorAreaIds.Contains(pr.ResearchAreaId)))
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
            project.Status = "Assigned";

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

}