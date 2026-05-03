using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayrollApprovalSystem.Api.DTOs.Approval;
using PayrollApprovalSystem.Api.Mappings;
using PayrollApprovalSystem.Application.Services;
using PayrollApprovalSystem.Domain.Entities;
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
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApprovalResponseDto>> ApprovePayroll([FromBody] ApprovePayrollRequestDto request)
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

        var approval = await _approvalRepository.GetByPayrollIdAsync(payroll.Id);
        var isNewApproval = approval is null;
        approval ??= new Approval(Guid.NewGuid(), payroll.Id);

        _approvalService.ApprovePayroll(payroll, approval);
        await _payrollRepository.UpdateAsync(payroll);

        if (isNewApproval)
            await _approvalRepository.AddAsync(approval);
        else
            await _approvalRepository.UpdateAsync(approval);

        return Ok(approval.ToDto(payroll));
    }
}
