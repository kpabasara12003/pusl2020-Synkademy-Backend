using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public StudentsController(AppDbContext context)
    {
        _context = context;
    }

    // GET api/students/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetStudent(int id)
    {
        var student = await _context.Students.FindAsync(id);
        if (student == null) return NotFound();

        var resp = new StudentResponse
        {
            Id = student.Id,
            StudentNumber = student.StudentNumber,
            FullName = student.FullName,
            Email = student.Email,
            CreatedAt = student.CreatedAt
        };

        return Ok(resp);
    }
}
