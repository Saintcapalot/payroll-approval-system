using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Repositories;

public class PayrollStructureRepository : IPayrollStructureRepository
{
    private readonly AppDbContext _context;

    public PayrollStructureRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PayrollStructure?> GetActiveByEmployeeIdAsync(Guid employeeId) =>
        await _context.PayrollStructures
            .FirstOrDefaultAsync(ps => ps.EmployeeId == employeeId && ps.IsActive);

    public async Task AddAsync(PayrollStructure payrollStructure)
    {
        await _context.PayrollStructures.AddAsync(payrollStructure);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(PayrollStructure payrollStructure)
    {
        _context.PayrollStructures.Update(payrollStructure);
        await _context.SaveChangesAsync();
    }
}
