using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using Synkademy.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;

    public AccountController(AppDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    [HttpPost("create/student")]
    public async Task<IActionResult> CreateStudent([FromBody] CreateStudentRequest request)
    {

        if (!await IsModuleLeader(request.ModuleLeaderId))
            return Unauthorized("Only ModuleLeader can create accounts.");

        if (string.IsNullOrWhiteSpace(request.StudentNumber) || request.StudentNumber.Length > 50)
            return BadRequest("Invalid StudentNumber.");
        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length > 100)
            return BadRequest("Invalid FullName.");
        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 100)
            return BadRequest("Invalid Email.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6 || request.Password.Length > 9)
            return BadRequest("Password must be 6 to 9 characters.");

        if (await _context.Students.AnyAsync(s => s.StudentNumber == request.StudentNumber))
            return Conflict("StudentNumber already exists.");
        if (await _context.Students.AnyAsync(s => s.Email == request.Email))
            return Conflict("Email already exists.");

        var student = new Student
        {
            StudentNumber = request.StudentNumber,
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password)
        };

        _context.Students.Add(student);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Student account created successfully.", student.Id });
    }


    [HttpPost("create/supervisor")]
    public async Task<IActionResult> CreateSupervisor([FromBody] CreateSupervisorRequest request)
    {
        if (!await IsModuleLeader(request.ModuleLeaderId))
            return Unauthorized("Only ModuleLeader can create accounts.");

        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length > 100)
            return BadRequest("Invalid FullName.");
        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 100)
            return BadRequest("Invalid Email.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6 || request.Password.Length > 9)
            return BadRequest("Password must be 6 to 9 characters.");

        if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
            return Conflict("Email already exists.");

        var supervisor = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = "Supervisor"
        };

        _context.Employees.Add(supervisor);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Supervisor account created successfully.", supervisor.Id });
    }

 
    [HttpPost("create/moduleleader")]
    public async Task<IActionResult> CreateModuleLeader([FromBody] CreateModuleLeaderRequest request)
    {
        if (!await IsModuleLeader(request.ModuleLeaderId))
            return Unauthorized("Only ModuleLeader can create accounts.");

        if (string.IsNullOrWhiteSpace(request.FullName) || request.FullName.Length > 100)
            return BadRequest("Invalid FullName.");
        if (string.IsNullOrWhiteSpace(request.Email) || request.Email.Length > 100)
            return BadRequest("Invalid Email.");
        if (string.IsNullOrWhiteSpace(request.Password) || request.Password.Length < 6 || request.Password.Length > 9)
            return BadRequest("Password must be 6 to 9 characters.");

        if (await _context.Employees.AnyAsync(e => e.Email == request.Email))
            return Conflict("Email already exists.");

        var moduleLeader = new Employee
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = "ModuleLeader"
        };

        _context.Employees.Add(moduleLeader);
        await _context.SaveChangesAsync();

        return Ok(new { message = "ModuleLeader account created successfully.", moduleLeader.Id });
    }

    private async Task<bool> IsModuleLeader(int moduleLeaderId)
    {
        var user = await _context.Employees.FirstOrDefaultAsync(u => u.Id == moduleLeaderId);
        return user != null && user.Role == "ModuleLeader";
    }
}