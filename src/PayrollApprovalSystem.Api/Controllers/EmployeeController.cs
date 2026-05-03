using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Employee;
using PayrollApprovalSystem.Domain.Interfaces;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmployeeController : ControllerBase
{
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public EmployeeController(
        IEmployeeRepository employeeRepository,
        IDepartmentRepository departmentRepository)
    {
        _employeeRepository = employeeRepository;
        _departmentRepository = departmentRepository;
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetEmployeeById(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
            return NotFound(new { message = "Employee not found." });

        return Ok(new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = $"{employee.FirstName} {employee.LastName}",
            Email = employee.Email,
            DepartmentId = employee.DepartmentId
        });
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllEmployees()
    {
        var employees = await _employeeRepository.GetAllAsync();
        var dtos = employees.Select(e => new EmployeeResponseDto
        {
            Id = e.Id,
            FullName = $"{e.FirstName} {e.LastName}",
            Email = e.Email,
            DepartmentId = e.DepartmentId
        }).ToList();

        return Ok(dtos);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeRequestDto request)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
            return BadRequest(new { message = "Department not found." });

        var employee = new Domain.Entities.Employee(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.Email,
            request.DepartmentId);

        await _employeeRepository.AddAsync(employee);

        return CreatedAtAction(
            nameof(GetEmployeeById),
            new { id = employee.Id },
            new EmployeeResponseDto
            {
                Id = employee.Id,
                FullName = $"{employee.FirstName} {employee.LastName}",
                Email = employee.Email,
                DepartmentId = employee.DepartmentId
            });
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateEmployee(Guid id, [FromBody] UpdateEmployeeRequestDto request)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
            return NotFound(new { message = "Employee not found." });

        var updated = new Domain.Entities.Employee(
            id,
            request.FirstName,
            request.LastName,
            request.Email,
            request.DepartmentId);

        if (!request.IsActive)
            updated.Deactivate();

        await _employeeRepository.UpdateAsync(updated);

        return Ok(new EmployeeResponseDto
        {
            Id = updated.Id,
            FullName = $"{updated.FirstName} {updated.LastName}",
            Email = updated.Email,
            DepartmentId = updated.DepartmentId
        });
    }
}
