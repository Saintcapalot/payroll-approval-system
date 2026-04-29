using PayrollApprovalSystem.Domain.Entities;

namespace PayrollApprovalSystem.Domain.Interfaces;

public interface IDepartmentRepository
{
    Task<Department?> GetByIdAsync(Guid id);
    Task<IReadOnlyList<Department>> GetAllAsync();
    Task AddAsync(Department department);
}
