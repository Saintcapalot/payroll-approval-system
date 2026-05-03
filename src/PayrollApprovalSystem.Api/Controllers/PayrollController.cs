using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Payroll;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Interfaces;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly PayrollGenerationService _payrollGenerationService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IPayrollStructureRepository _payrollStructureRepository;
    private readonly IPayrollRepository _payrollRepository;

    public PayrollController(
        PayrollGenerationService payrollGenerationService,
        IEmployeeRepository employeeRepository,
        IPayrollStructureRepository payrollStructureRepository,
        IPayrollRepository payrollRepository)
    {
        _payrollGenerationService = payrollGenerationService;
        _employeeRepository = employeeRepository;
        _payrollStructureRepository = payrollStructureRepository;
        _payrollRepository = payrollRepository;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayrollResponseDto>> GeneratePayroll(GeneratePayrollRequestDto request)
    {
        var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
        if (employee is null)
            return BadRequest(new { message = "Employee not found.", type = "DomainError" });

        var payrollStructure = await _payrollStructureRepository.GetActiveByEmployeeIdAsync(request.EmployeeId);
        if (payrollStructure is null)
            return BadRequest(new { message = "No active payroll structure found for employee.", type = "DomainError" });

        var payrollAlreadyExists = await _payrollRepository.ExistsForMonthAsync(
            request.EmployeeId, request.Month, request.Year);

        try
        {
            var payroll = _payrollGenerationService.GeneratePayroll(
                employee,
                payrollStructure,
                request.Month,
                request.Year,
                payrollAlreadyExists);

            await _payrollRepository.AddAsync(payroll);

            return Ok(payroll.ToDto());
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { message = ex.Message, type = "DomainError" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayrollResponseDto>> GetPayrollById(Guid id)
    {
        var payroll = await _payrollRepository.GetByIdAsync(id);
        if (payroll is null)
            return NotFound(new { message = "Payroll not found." });

        return Ok(payroll.ToDto());
    }

    [HttpGet("employee/{employeeId}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PayrollResponseDto>>> GetPayrollsByEmployee(Guid employeeId)
    {
        var payrolls = await _payrollRepository.GetByEmployeeIdAsync(employeeId);
        return Ok(payrolls.Select(p => p.ToDto()));
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<PayrollResponseDto>>> GetAllPayrolls()
    {
        var payrolls = await _payrollRepository.GetAllAsync();
        return Ok(payrolls.Select(p => p.ToDto()));
    }
}
