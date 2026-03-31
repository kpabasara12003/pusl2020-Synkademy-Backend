using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Synkademy.Data;
using Synkademy.DTOs;
using Synkademy.Models;
using Synkademy.Services;

namespace Synkademy.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly PasswordService _passwordService;

    public AuthController(AppDbContext context, PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }


    [HttpPost("student-login")]
    public async Task<IActionResult> StudentLogin(LoginRequest request)
    {
        var student = await _context.Students
            .FirstOrDefaultAsync(s => s.Email == request.Email);

        if (student == null)
            return Unauthorized("Invalid email or password");

        var isValid = _passwordService.VerifyPassword(request.Password, student.PasswordHash);

        if (!isValid)
            return Unauthorized("Invalid email or password");

        var response = new LoginResponse
        {
            Id = student.Id,
            FullName = student.FullName,
            StudentNumber = student.StudentNumber,
            Email = student.Email,
            Role = "Student"
        };

        return Ok(response);
    }


    [HttpPost("supervisor-login")]
    public async Task<IActionResult> SupervisorLogin(LoginRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == request.Email && e.Role == "Supervisor");

        if (employee == null)
            return Unauthorized("Invalid email or password");

        var isValid = _passwordService.VerifyPassword(request.Password, employee.PasswordHash);

        if (!isValid)
            return Unauthorized("Invalid email or password");

        var response = new LoginResponse
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Role = employee.Role
        };

        return Ok(response);
    }

    [HttpPost("moduleleader-login")]
    public async Task<IActionResult> ModuleLeaderLogin(LoginRequest request)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Email == request.Email && e.Role == "ModuleLeader");

        if (employee == null)
            return Unauthorized("Invalid email or password");

        var isValid = _passwordService.VerifyPassword(request.Password, employee.PasswordHash);

        if (!isValid)
            return Unauthorized("Invalid email or password");

        var response = new LoginResponse
        {
            Id = employee.Id,
            FullName = employee.FullName,
            Email = employee.Email,
            Role = employee.Role
        };

        return Ok(response);
    }
    
}