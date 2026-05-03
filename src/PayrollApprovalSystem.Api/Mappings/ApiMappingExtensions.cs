using PayrollApprovalSystem.Api.DTOs.Approval;
using PayrollApprovalSystem.Api.DTOs.Employee;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Api.DTOs.Payroll;
using PayrollApprovalSystem.Api.DTOs.Payslip;

namespace PayrollApprovalSystem.Api.Mappings;

public static class ApiMappingExtensions
{
    public static PayrollResponseDto ToDto(this Payroll payroll)
    {
        return new PayrollResponseDto
        {
            Id = payroll.Id,
            EmployeeId = payroll.EmployeeId,
            Month = payroll.Month,
            Year = payroll.Year,
            BaseSalary = payroll.BaseSalary,
            Bonus = payroll.Bonus,
            Deductions = payroll.Deductions,
            TotalAmount = payroll.TotalAmount,
            Status = payroll.Status.ToString()
        };
    }

    public static ApprovalResponseDto ToDto(this Approval approval, Payroll payroll)
    {
        return new ApprovalResponseDto
        {
            ApprovalId = approval.Id,
            PayrollId = approval.PayrollId,
            ApprovalStatus = approval.Status.ToString(),
            PayrollStatus = payroll.Status.ToString(),
            ReviewedAt = approval.ReviewedAt
        };
    }

    public static PayslipResponseDto ToDto(this Payslip payslip)
    {
        return new PayslipResponseDto
        {
            PayslipId = payslip.Id,
            PayrollId = payslip.PayrollId
        };
    }

    public static EmployeeResponseDto ToDto(this Employee employee)
    {
        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FullName = $"{employee.FirstName} {employee.LastName}".Trim(),
            Email = employee.Email,
            DepartmentId = employee.DepartmentId
        };
    }
}
