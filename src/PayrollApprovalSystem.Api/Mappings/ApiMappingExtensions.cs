using PayrollApprovalSystem.Api.DTOs.Payroll;
using PayrollApprovalSystem.Domain.Entities;

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
}