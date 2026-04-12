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
                // 1. Fetch data from DB
                var projects = await _context.Projects
                    .Include(p => p.Student)
                    .Include(p => p.Supervisor)
                    .Include(p => p.ProjectResearchAreas)
                        .ThenInclude(pra => pra.ResearchArea)
                    .ToListAsync();

                // 2. Safely calculate KPIs (Handling NULL statuses)
                var totalProposals = projects.Count;
                var matchedProjects = projects.Count(p => string.Equals(p.Status, "Matched", StringComparison.OrdinalIgnoreCase));

                // If status is null, we can assume it's pending review
                var pendingReview = projects.Count(p => string.Equals(p.Status, "Pending", StringComparison.OrdinalIgnoreCase) || string.IsNullOrWhiteSpace(p.Status));

                // 3. Map to DTO safely
                var projectDtos = projects.Select(p => new ProjectDirectoryDto
                {
                    ProjectId = p.Id,
                    Title = p.Title ?? "Untitled Project",
                    Status = string.IsNullOrWhiteSpace(p.Status) ? "Pending" : p.Status,

                    // Safely check if ProjectResearchAreas is null before selecting
                    ResearchAreas = p.ProjectResearchAreas != null
                        ? p.ProjectResearchAreas.Select(pra => pra.ResearchArea.Name).ToList()
                        : new List<string>(),

                    // Blind Match Logic
                    StudentName = string.Equals(p.Status, "Matched", StringComparison.OrdinalIgnoreCase) && p.Student != null
                                  ? $"Revealed ({p.Student.FullName})"
                                  : "Hidden (Blind Phase)",

                    SupervisorName = p.Supervisor != null ? p.Supervisor.FullName : "Unassigned"

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

                // 4. Return the data
                var response = new DashboardResponseDto
                {
                    TotalProposals = totalProposals,
                    MatchedProjects = matchedProjects,
                    PendingReview = pendingReview,
                    Projects = projectDtos,
                    AvailableResearchAreas = researchAreas,
                    AvailableSupervisors = supervisors
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                // If it crashes, this will print the EXACT reason in your Visual Studio output window!
                Console.WriteLine($"\n--- API CRASHED ---\n{ex.Message}\n{ex.StackTrace}\n-------------------\n");
                return StatusCode(500, "Internal server error. Check Visual Studio console.");
            }
        }
    }
}