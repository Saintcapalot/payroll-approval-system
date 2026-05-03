using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Payroll;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Exceptions;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayrollResponseDto>> GeneratePayroll([FromBody] GeneratePayrollRequestDto request)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(request.EmployeeId);
            if (employee is null)
            {
                return NotFound(new
                {
                    message = $"Employee with id '{request.EmployeeId}' was not found.",
                    type = "NotFound"
                });
            }

            var payrollStructure = await _payrollStructureRepository.GetActiveByEmployeeIdAsync(request.EmployeeId);
            if (payrollStructure is null)
            {
                return NotFound(new
                {
                    message = $"Active payroll structure for employee '{request.EmployeeId}' was not found.",
                    type = "NotFound"
                });
            }

            var payrollAlreadyExists = await _payrollRepository.ExistsForMonthAsync(
                request.EmployeeId,
                request.Month,
                request.Year);

            var payroll = _payrollGenerationService.GeneratePayroll(
                employee,
                payrollStructure,
                request.Month,
                request.Year,
                payrollAlreadyExists);

            await _payrollRepository.AddAsync(payroll);

            return Ok(payroll.ToDto());
        }
        catch (DomainException ex)
        {
            return BadRequest(new
            {
                message = ex.Message,
                type = "DomainError"
            });
        }
    }
}
