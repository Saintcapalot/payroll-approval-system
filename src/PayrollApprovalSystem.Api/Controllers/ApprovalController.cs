using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Approval;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Interfaces;

namespace PayrollApprovalSystem.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApprovalController : ControllerBase
{
    private readonly ApprovalService _approvalService;
    private readonly IPayrollRepository _payrollRepository;
    private readonly IApprovalRepository _approvalRepository;

    public ApprovalController(
        ApprovalService approvalService,
        IPayrollRepository payrollRepository,
        IApprovalRepository approvalRepository)
    {
        _approvalService = approvalService;
        _payrollRepository = payrollRepository;
        _approvalRepository = approvalRepository;
    }

    [HttpPost("approve")]
    [Authorize(Roles = "Manager,Admin")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalResponseDto>> ApprovePayroll([FromBody] ApprovePayrollRequestDto request)
    {
        var payroll = await _payrollRepository.GetByIdAsync(request.PayrollId);
        if (payroll is null)
            return NotFound(new { message = "Payroll not found." });

        var existingApproval = await _approvalRepository.GetByPayrollIdAsync(request.PayrollId);
        if (existingApproval is not null)
            return BadRequest(new { message = "Payroll already has an approval.", type = "DomainError" });

        try
        {
            var approval = new Domain.Entities.Approval(Guid.NewGuid(), payroll.Id);
            _approvalService.ApprovePayroll(payroll, approval);

            await _approvalRepository.AddAsync(approval);
            await _payrollRepository.UpdateAsync(payroll);

            return Ok(approval.ToDto(payroll));
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
    public async Task<ActionResult<ApprovalResponseDto>> GetApprovalById(Guid id)
    {
        var approval = await _approvalRepository.GetByIdAsync(id);
        if (approval is null)
            return NotFound(new { message = "Approval not found." });

        var payroll = await _payrollRepository.GetByIdAsync(approval.PayrollId);
        if (payroll is null)
            return NotFound(new { message = "Associated payroll not found." });

        return Ok(approval.ToDto(payroll));
    }

    [HttpGet("payroll/{payrollId}")]
    [Authorize(Roles = "Admin,Manager")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApprovalResponseDto>> GetApprovalByPayrollId(Guid payrollId)
    {
        var approval = await _approvalRepository.GetByPayrollIdAsync(payrollId);
        if (approval is null)
            return NotFound(new { message = "No approval found for this payroll." });

        var payroll = await _payrollRepository.GetByIdAsync(payrollId);
        if (payroll is null)
            return NotFound(new { message = "Associated payroll not found." });

        return Ok(approval.ToDto(payroll));
    }
}
