using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Payslip;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Api.Services;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Enums;
using PayrollApprovalSystem.Domain.Exceptions;
using PayrollApprovalSystem.Domain.Interfaces;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PayslipController : ControllerBase
{
    private readonly PayslipService _payslipService;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IPayslipRepository _payslipRepository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IApprovalRepository _approvalRepository;
    private readonly PayslipPdfService _payslipPdfService;

    public PayslipController(
        PayslipService payslipService,
        IPayrollRepository payrollRepository,
        IPayslipRepository payslipRepository,
        IEmployeeRepository employeeRepository,
        IApprovalRepository approvalRepository,
        PayslipPdfService payslipPdfService)
    {
        _payslipService = payslipService;
        _payrollRepository = payrollRepository;
        _payslipRepository = payslipRepository;
        _employeeRepository = employeeRepository;
        _approvalRepository = approvalRepository;
        _payslipPdfService = payslipPdfService;
    }

    [HttpPost("generate")]
    [Authorize(Roles = "Employee,Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PayslipResponseDto>> GeneratePayslip([FromBody] GeneratePayslipRequestDto request)
    {
        try
        {
            var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId);
            if (payroll is null)
            {
                return NotFound(new
                {
                    message = $"Payroll with id '{request.PayrollId}' was not found.",
                    type = "NotFound"
                });
            }

            var existingPayslip = await _payslipRepository.GetByPayrollIdAsync(request.PayrollId);
            if (existingPayslip is not null)
                return Ok(existingPayslip.ToDto());

            var payslip = _payslipService.GeneratePayslip(payroll);
            await _payslipRepository.AddAsync(payslip);

            return Ok(payslip.ToDto());
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

    [HttpGet("{payrollId:guid}/pdf")]
    [Authorize(Roles = "Employee,Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetPayslipPdf(Guid payrollId)
    {
        var payroll = await _payrollRepository.GetByIdAsync(payrollId);
        if (payroll is null)
        {
            return NotFound(new
            {
                message = $"Payroll with id '{payrollId}' was not found.",
                type = "NotFound"
            });
        }

        if (payroll.Status != PayrollStatus.Approved)
        {
            return BadRequest(new
            {
                message = "Payslip PDF can only be generated for approved payroll.",
                type = "DomainError"
            });
        }

        var employee = await _employeeRepository.GetByIdAsync(payroll.EmployeeId);
        if (employee is null)
        {
            return NotFound(new
            {
                message = $"Employee with id '{payroll.EmployeeId}' was not found.",
                type = "NotFound"
            });
        }

        var approval = await _approvalRepository.GetByPayrollIdAsync(payrollId);
        var existingPayslip = await _payslipRepository.GetByPayrollIdAsync(payrollId);

        var pdfBytes = _payslipPdfService.GeneratePdf(
            employee,
            payroll,
            approval?.Status.ToString() ?? payroll.Status.ToString(),
            existingPayslip?.GeneratedAt ?? DateTime.UtcNow);

        return File(pdfBytes, "application/pdf", "payslip.pdf");
    }
}
