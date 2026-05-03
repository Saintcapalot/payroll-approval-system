using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Employee;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Domain.Entities;
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
    public async Task<ActionResult<EmployeeResponseDto>> GetEmployeeById(Guid id)
    {
        var employee = await _employeeRepository.GetByIdAsync(id);
        if (employee is null)
        {
            return NotFound(new
            {
                message = $"Employee with id '{id}' was not found.",
                type = "NotFound"
            });
        }

        return Ok(employee.ToDto());
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<EmployeeResponseDto>>> GetAllEmployees()
    {
        var employees = await _employeeRepository.GetAllAsync();
        return Ok(employees.Select(employee => employee.ToDto()).ToList());
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EmployeeResponseDto>> CreateEmployee([FromBody] CreateEmployeeRequestDto request)
    {
        var department = await _departmentRepository.GetByIdAsync(request.DepartmentId);
        if (department is null)
        {
            return NotFound(new
            {
                message = $"Department with id '{request.DepartmentId}' was not found.",
                type = "NotFound"
            });
        }

        var employee = new Employee(
            Guid.NewGuid(),
            request.FirstName,
            request.LastName,
            request.Email,
            request.DepartmentId);

        await _employeeRepository.AddAsync(employee);

        return CreatedAtAction(nameof(GetEmployeeById), new { id = employee.Id }, employee.ToDto());
    }
}
