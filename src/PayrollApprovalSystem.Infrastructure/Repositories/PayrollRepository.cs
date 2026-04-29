using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Repositories;

public class PayrollRepository : IPayrollRepository
{
    private readonly AppDbContext _context;

    public PayrollRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Payroll?> GetByIdAsync(Guid id) =>
        await _context.Payrolls.FindAsync(id);

    public async Task<IReadOnlyList<Payroll>> GetAllAsync() =>
        await _context.Payrolls.ToListAsync();

    public async Task<IReadOnlyList<Payroll>> GetByEmployeeIdAsync(Guid employeeId) =>
        await _context.Payrolls.Where(p => p.EmployeeId == employeeId).ToListAsync();

    public async Task<bool> ExistsForMonthAsync(Guid employeeId, int month, int year) =>
        await _context.Payrolls.AnyAsync(p => p.EmployeeId == employeeId && p.Month == month && p.Year == year);

    public async Task AddAsync(Payroll payroll)
    {
        await _context.Payrolls.AddAsync(payroll);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Payroll payroll)
    {
        _context.Payrolls.Update(payroll);
        await _context.SaveChangesAsync();
    }
}
