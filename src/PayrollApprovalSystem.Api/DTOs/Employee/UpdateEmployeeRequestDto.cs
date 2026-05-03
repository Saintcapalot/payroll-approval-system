namespace PayrollApprovalSystem.Api.DTOs.Employee;

public class UpdateEmployeeRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public bool IsActive { get; set; } = true;
}
