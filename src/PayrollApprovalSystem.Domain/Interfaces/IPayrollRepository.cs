using PayrollApprovalSystem.Domain.Entities;

namespace PayrollApprovalSystem.Domain.Interfaces;

public interface IPayrollRepository
{
    Task<Payroll?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Payroll>> GetAllAsync();
    Task<IReadOnlyList<Payroll>> GetByEmployeeIdAsync(Guid employeeId);
    Task<bool> ExistsForMonthAsync(Guid employeeId, int month, int year);
    Task AddAsync(Payroll payroll);
    Task UpdateAsync(Payroll payroll);
}
