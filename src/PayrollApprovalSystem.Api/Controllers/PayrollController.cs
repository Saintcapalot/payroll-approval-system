using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Payroll;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Exceptions;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayrollController : ControllerBase
{
    private readonly PayrollGenerationService _payrollGenerationService;

    public PayrollController(PayrollGenerationService payrollGenerationService)
    {
        _payrollGenerationService = payrollGenerationService;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PayrollResponseDto> GeneratePayroll(GeneratePayrollRequestDto request)    {
        try
        {
            // TODO: Replace manual Employee creation with database/repository lookup.
            // TODO: Replace manual PayrollStructure creation with database/repository lookup.
            // TODO: Persist generated Payroll through Infrastructure layer.

            var employee = new Employee(
                request.EmployeeId,
                request.FirstName,
                request.LastName,
                request.Email,
                request.DepartmentId);

            var payrollStructure = new PayrollStructure(
                Guid.NewGuid(),
                request.EmployeeId,
                request.BaseSalary,
                request.Bonus,
                request.Deductions);

            var payroll = _payrollGenerationService.GeneratePayroll(
                employee,
                payrollStructure,
                request.Month,
                request.Year,
                request.PayrollAlreadyExists);

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
