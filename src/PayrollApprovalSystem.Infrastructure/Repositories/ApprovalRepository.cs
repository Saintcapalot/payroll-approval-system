using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Repositories;

public class ApprovalRepository : IApprovalRepository
{
    private readonly AppDbContext _context;

    public ApprovalRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Approval?> GetByIdAsync(Guid id) =>
        await _context.Approvals.FindAsync(id);

    public async Task<Approval?> GetByPayrollIdAsync(Guid payrollId) =>
        await _context.Approvals.FirstOrDefaultAsync(a => a.PayrollId == payrollId);

    public async Task AddAsync(Approval approval)
    {
        await _context.Approvals.AddAsync(approval);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Approval approval)
    {
        _context.Approvals.Update(approval);
        await _context.SaveChangesAsync();
    }
}
