using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;

namespace Synkademy.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TagsController(AppDbContext context)
        {
            _context = context;
        }

        // GET api/tags
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _context.Tags
                .OrderBy(t => t.Name)
                .Select(t => new
                {
                    t.Id,
                    t.Name
                })
                .ToListAsync();

            return Ok(list);
        }

        // POST api/tags
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTagRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Name))
                return BadRequest("Tag name is required.");

            var exists = await _context.Tags
                .AnyAsync(t => t.Name == request.Name);

            if (exists)
                return Conflict("Tag already exists.");

            var tag = new Tag
            {
                Name = request.Name
            };

            _context.Tags.Add(tag);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tag created.",
                id = tag.Id
            });
        }
    }
}