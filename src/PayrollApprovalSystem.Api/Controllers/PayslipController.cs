using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Payslip;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Interfaces;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayslipController : ControllerBase
{
    private readonly PayslipService _payslipService;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IPayslipRepository _payslipRepository;

    public PayslipController(
        PayslipService payslipService,
        IPayrollRepository payrollRepository,
        IPayslipRepository payslipRepository)
    {
        _payslipService = payslipService;
        _payrollRepository = payrollRepository;
        _payslipRepository = payslipRepository;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Employee,Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayslipResponseDto>> GeneratePayslip([FromBody] GeneratePayslipRequestDto request)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId);
        if (payroll is null)
            return NotFound(new { message = "Payroll not found." });

        try
        {
            var payslip = _payslipService.GeneratePayslip(payroll);
            await _payslipRepository.AddAsync(payslip);

            return Ok(payslip.ToDto());
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return BadRequest(new { message = ex.Message, type = "DomainError" });
        }
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayslipResponseDto>> GetPayslipById(Guid id)
    {
        var payslip = await _payslipRepository.GetByIdAsync(id);
        if (payslip is null)
            return NotFound(new { message = "Payslip not found." });

        return Ok(payslip.ToDto());
    }

    [HttpGet("payroll/{payrollId}")]
    [Authorize(Roles = "Admin,Manager,Employee")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PayslipResponseDto>> GetPayslipByPayrollId(Guid payrollId)
    {
        var payslip = await _payslipRepository.GetByPayrollIdAsync(payrollId);
        if (payslip is null)
            return NotFound(new { message = "No payslip found for this payroll." });

        return Ok(payslip.ToDto());
    }
}
