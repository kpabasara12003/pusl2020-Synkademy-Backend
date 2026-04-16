using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using System.Threading.Tasks;

namespace Synkademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MetadataController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MetadataController(AppDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // 1. GET ENDPOINTS (Fetch Data)
        // ==========================================
        [HttpGet("tags")]
        public async Task<IActionResult> GetTags()
        {
            var tags = await _context.Tags
                .Select(t => new MetadataDto { Id = t.Id, Name = t.Name })
                .ToListAsync();
            return Ok(tags);
        }

        [HttpGet("researchareas")]
        public async Task<IActionResult> GetResearchAreas()
        {
            var areas = await _context.ResearchAreas
                .Select(r => new MetadataDto { Id = r.Id, Name = r.Name })
                .ToListAsync();
            return Ok(areas);
        }

        // ==========================================
        // 2. POST ENDPOINTS (Create Data)
        // ==========================================
        [HttpPost("tags")]
        public async Task<IActionResult> CreateTag([FromBody] CreateMetadataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");
            if (await _context.Tags.AnyAsync(t => t.Name.ToLower() == request.Name.ToLower()))
                return Conflict("Tag already exists.");

            var newTag = new Tag { Name = request.Name };
            _context.Tags.Add(newTag);
            await _context.SaveChangesAsync();

            return Ok(new MetadataDto { Id = newTag.Id, Name = newTag.Name });
        }

        [HttpPost("researchareas")]
        public async Task<IActionResult> CreateResearchArea([FromBody] CreateMetadataRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name)) return BadRequest("Name is required.");
            if (await _context.ResearchAreas.AnyAsync(r => r.Name.ToLower() == request.Name.ToLower()))
                return Conflict("Research Area already exists.");

            var newArea = new ResearchArea { Name = request.Name };
            _context.ResearchAreas.Add(newArea);
            await _context.SaveChangesAsync();

            return Ok(new MetadataDto { Id = newArea.Id, Name = newArea.Name });
        }

        // ==========================================
        // 3. DELETE ENDPOINTS (Remove Data)
        // ==========================================
        [HttpDelete("tags/{id}")]
        public async Task<IActionResult> DeleteTag(int id)
        {
            var tag = await _context.Tags.FindAsync(id);
            if (tag == null) return NotFound("Tag not found.");

            try
            {
                _context.Tags.Remove(tag);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Tag deleted." });
            }
            catch (DbUpdateException)
            {
                // If it crashes here, it means a Project is currently using this Tag!
                return BadRequest("Cannot delete this tag because it is currently assigned to a project.");
            }
        }

        [HttpDelete("researchareas/{id}")]
        public async Task<IActionResult> DeleteResearchArea(int id)
        {
            var area = await _context.ResearchAreas.FindAsync(id);
            if (area == null) return NotFound("Research Area not found.");

            try
            {
                _context.ResearchAreas.Remove(area);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Research Area deleted." });
            }
            catch (DbUpdateException)
            {
                return BadRequest("Cannot delete this area because it is currently assigned to a project or supervisor.");
            }
        }
    }
}