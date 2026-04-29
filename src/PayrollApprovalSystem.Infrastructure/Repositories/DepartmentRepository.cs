using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Repositories;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Department?> GetByIdAsync(Guid id) =>
        await _context.Departments.FindAsync(id);

    public async Task<IReadOnlyList<Department>> GetAllAsync() =>
        await _context.Departments.ToListAsync();

    public async Task AddAsync(Department department)
    {
        await _context.Departments.AddAsync(department);
        await _context.SaveChangesAsync();
    }
}
