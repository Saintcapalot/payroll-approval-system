using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Repositories;

public class PayslipRepository : IPayslipRepository
{
    private readonly AppDbContext _context;

    public PayslipRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payslip?> GetByIdAsync(Guid id) =>
        await _context.Payslips.FindAsync(id);

    public async Task<Payslip?> GetByPayrollIdAsync(Guid payrollId) =>
        await _context.Payslips.FirstOrDefaultAsync(p => p.PayrollId == payrollId);

    public async Task AddAsync(Payslip payslip)
    {
        await _context.Payslips.AddAsync(payslip);
        await _context.SaveChangesAsync();
    }
}
