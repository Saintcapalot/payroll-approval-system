namespace PayrollApprovalSystem.Api.DTOs.Payroll;

public class GeneratePayrollRequestDto
{
    public Guid EmployeeId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }

    public decimal BaseSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deductions { get; set; }

    public int Month { get; set; }
    public int Year { get; set; }

    public bool PayrollAlreadyExists { get; set; }
}