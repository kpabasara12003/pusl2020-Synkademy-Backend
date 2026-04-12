using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;

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

    [HttpGet("{supervisorId}/projects")]
    public async Task<IActionResult> GetProjects(
        int supervisorId,
        [FromQuery] ProjectQueryDto query)
    {
        try
        {
            var supervisor = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == supervisorId && e.Role == "Supervisor");

            if (supervisor == null)
                return NotFound(new { message = "Supervisor not found." });

      
            var supervisorAreaIds = await _context.SupervisorResearchAreas
                .Where(x => x.SupervisorId == supervisorId)
                .Select(x => x.ResearchAreaId)
                .ToListAsync();

            if (!supervisorAreaIds.Any())
                return BadRequest(new { message = "No research areas assigned to supervisor." });

            var projectsQuery = _context.Projects
                .Where(p => p.Status == "Pending") 
                .Where(p => p.SupervisorId == null) 
                .Where(p => p.ProjectResearchAreas!
                    .Any(pr => supervisorAreaIds.Contains(pr.ResearchAreaId)))
                .AsQueryable();


            if (!string.IsNullOrWhiteSpace(query.Search))
            {
                var search = query.Search.ToLower();

                projectsQuery = projectsQuery.Where(p =>
                    p.Title.ToLower().Contains(search) ||
                    (p.ShortDescription != null && p.ShortDescription.ToLower().Contains(search)) ||
                    (p.TechStack != null && p.TechStack.ToLower().Contains(search))
                );
            }


            if (query.ResearchAreaIds != null && query.ResearchAreaIds.Any())
            {
                projectsQuery = projectsQuery.Where(p =>
                    p.ProjectResearchAreas!
                        .Any(pr => query.ResearchAreaIds.Contains(pr.ResearchAreaId)));
            }

            var result = await projectsQuery
                .Select(p => new ProjectListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    ShortDescription = p.ShortDescription,
                    TechStack = p.TechStack,

                    ResearchAreas = p.ProjectResearchAreas!
                        .Select(pr => pr.ResearchArea.Name)
                        .ToList()
                })
                .ToListAsync();

            return Ok(new
            {
                count = result.Count,
                data = result
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = "An error occurred.",
                error = ex.Message
            });
        }
    }
}