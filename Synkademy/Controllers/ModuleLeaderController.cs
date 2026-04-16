using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using System.Linq;
using System.Threading.Tasks;

namespace Synkademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ModuleLeaderController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ModuleLeaderController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardData()
        {
            try
            {
                var projects = await _context.Projects
                    .Include(p => p.Student)
                    .Include(p => p.Supervisor)
                    .Include(p => p.ProjectResearchAreas)
                        .ThenInclude(pra => pra.ResearchArea)
                    .Include(p => p.Tags)
                        .ThenInclude(pt => pt.Tag)
                    .ToListAsync();

                var totalProposals = projects.Count;
                var matchedProjects = projects.Count(p => string.Equals(p.Status, "Matched", StringComparison.OrdinalIgnoreCase));

                var pendingReview = projects.Count(p => string.Equals(p.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(p.Status));

                var projectDtos = projects.Select(p => new ProjectDirectoryDto
                {
                    ProjectId = p.Id,
                    Title = p.Title ?? "Untitled Project",
                    Status = string.IsNullOrWhiteSpace(p.Status) ? "Pending" : p.Status,

                    ResearchAreas = p.ProjectResearchAreas != null
                        ? p.ProjectResearchAreas.Select(pra => pra.ResearchArea.Name).ToList()
                        : new List<string>(),

                    Tags = p.Tags != null
                        ? p.Tags.Select(pt => pt.Tag.Name).ToList()
                        : new List<string>(),

                    StudentName = string.Equals(p.Status, "Matched", StringComparison.OrdinalIgnoreCase) && p.Student != null
                                  ? $"Revealed ({p.Student.FullName})"
                                  : "Hidden (Blind Phase)",

                    SupervisorName = p.Supervisor != null ? p.Supervisor.FullName : "Unassigned",

                    TechStack = p.TechStack,
                    Abstract = p.Abstract,
                   

                }).ToList();

                var researchAreas = await _context.ResearchAreas
            .Select(r => r.Name)
            .ToListAsync();

                var supervisors = await _context.Employees
            .Where(e => e.Role == "Supervisor")
            .Select(e => new SupervisorDropdownDto
            {
                Id = e.Id,
                Name = e.FullName
            })
            .ToListAsync();

                var response = new DashboardResponseDto
                {
                    TotalProposals = totalProposals,
                    MatchedProjects = matchedProjects,
                    PendingReview = pendingReview,
                    Projects = projectDtos,
                    AvailableResearchAreas = researchAreas,
                    AvailableSupervisors = supervisors,
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- API CRASHED ---\n{ex.Message}\n{ex.StackTrace}\n-------------------\n");
                return StatusCode(500, "Internal server error. Check Visual Studio console.");
            }
        }

        [HttpPost("assign")]
        public async Task<IActionResult> AssignSupervisor([FromBody] AssignSupervisorRequest request)
        {
            try
            {
                var project = await _context.Projects.FindAsync(request.ProjectId);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                var supervisor = await _context.Employees.FindAsync(request.SupervisorId);
                if (supervisor == null || supervisor.Role != "Supervisor")
                {
                    return BadRequest(new { message = "Invalid supervisor selected." });
                }

                project.SupervisorId = request.SupervisorId;
                project.Status = "Matched"; 

                await _context.SaveChangesAsync();

                return Ok(new { message = "Assignment saved successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- ASSIGNMENT CRASH ---\n{ex.Message}\n{ex.StackTrace}\n-------------------\n");
                return StatusCode(500, new { message = "Internal server error while assigning." });
            }
        }
        [HttpPost("break-match")]
        public async Task<IActionResult> BreakMatch([FromBody] BreakMatchRequest request)
        {
            try
            {
                var project = await _context.Projects.FindAsync(request.ProjectId);
                if (project == null)
                {
                    return NotFound(new { message = "Project not found." });
                }

                project.SupervisorId = null;
                project.Status = "Pending";

                await _context.SaveChangesAsync();

                return Ok(new { message = "Match broken successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n--- BREAK MATCH CRASH ---\n{ex.Message}\n{ex.StackTrace}\n-------------------\n");
                return StatusCode(500, new { message = "Internal server error while breaking match." });
            }
        }
    }
}