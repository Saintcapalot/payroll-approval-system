using PayrollApprovalSystem.Domain.Entities;

namespace PayrollApprovalSystem.Domain.Interfaces;

public interface IApprovalRepository
{
    Task<Approval?> GetByIdAsync(Guid id);
    Task<Approval?> GetByPayrollIdAsync(Guid payrollId);
    Task AddAsync(Approval approval);
    Task UpdateAsync(Approval approval);
}
