using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using Synkademy.Services;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using static Synkademy.DTOs.AccountDTO;

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

    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        // 1. Get Students
        var students = await _context.Students
            .Select(s => new UserDto
            {
                Id = s.Id,
                Name = s.FullName,
                Email = s.Email,
                Role = "student"
            }).ToListAsync();

        // 2. Get Employees (Supervisors and Module Leaders)
        var employees = await _context.Employees
            .Select(e => new UserDto
            {
                Id = e.Id,
                Name = e.FullName,
                Email = e.Email,
                Role = e.Role.ToLower() // Normalizes "ModuleLeader" to "moduleleader" for JS
            }).ToListAsync();

        // 3. Combine and return
        var allUsers = students.Concat(employees).ToList();
        return Ok(allUsers);
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest request)
    {
        try
        {
            if (request.Role == "student")
            {
                var student = await _context.Students.FindAsync(request.Id);
                if (student == null) return NotFound("Student not found.");

                // Check for email duplicates (excluding themselves)
                if (await _context.Students.AnyAsync(s => s.Email == request.Email && s.Id != request.Id))
                    return Conflict("Email is already in use by another student.");

                student.FullName = request.FullName;
                student.Email = request.Email;

                // Only update password if they typed a new one
                if (!string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    if (request.NewPassword.Length < 6 || request.NewPassword.Length > 9)
                        return BadRequest("Password must be 6 to 9 characters.");

                    student.PasswordHash = _passwordService.HashPassword(request.NewPassword);
                }
            }
            else // It's a supervisor or module leader
            {
                var employee = await _context.Employees.FindAsync(request.Id);
                if (employee == null) return NotFound("Employee not found.");

                if (await _context.Employees.AnyAsync(e => e.Email == request.Email && e.Id != request.Id))
                    return Conflict("Email is already in use by another employee.");

                employee.FullName = request.FullName;
                employee.Email = request.Email;

                if (!string.IsNullOrWhiteSpace(request.NewPassword))
                {
                    if (request.NewPassword.Length < 6 || request.NewPassword.Length > 9)
                        return BadRequest("Password must be 6 to 9 characters.");

                    employee.PasswordHash = _passwordService.HashPassword(request.NewPassword);
                }
            }

            await _context.SaveChangesAsync();
            return Ok(new { message = "User updated successfully." });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n--- UPDATE CRASH ---\n{ex.Message}\n-------------------\n");
            return StatusCode(500, "Internal server error.");
        }
    }
}