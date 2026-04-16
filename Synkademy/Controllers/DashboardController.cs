using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Synkademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        // 1. Count Relevant Proposals
        [HttpGet("{supervisorId}/relevant-count")]
        public async Task<ActionResult<int>> GetRelevantCount(int supervisorId)
        {
            var supervisorAreaIds = await _context.SupervisorResearchAreas
                .Where(sra => sra.SupervisorId == supervisorId)
                .Select(sra => sra.ResearchAreaId)
                .ToListAsync();

            var count = await _context.Projects
                .Where(p => p.Status == "Pending" && p.SupervisorId == null)
                .Where(p => p.ProjectResearchAreas.Any(pra => supervisorAreaIds.Contains(pra.ResearchAreaId)))
                .CountAsync();

            return Ok(count);
        }

        // 2. Count Pending Interests
        [HttpGet("{supervisorId}/interests-count")]
        public async Task<ActionResult<int>> GetInterestsCount(int supervisorId)
        {
            var count = await _context.Projects
                .Where(p => p.Status == "Pending" && p.Interests.Any(i => i.SupervisorId == supervisorId))
                .CountAsync();

            return Ok(count);
        }

        // 3. Count Matched Projects
        [HttpGet("{supervisorId}/assigned-count")]
        public async Task<ActionResult<int>> GetAssignedCount(int supervisorId)
        {
            var count = await _context.Projects
                .Where(p => p.SupervisorId == supervisorId)
                .CountAsync();

            return Ok(count);
        }
    }
}