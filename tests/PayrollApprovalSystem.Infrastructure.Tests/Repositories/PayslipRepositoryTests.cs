using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Persistence;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class PayslipRepositoryTests : TestBase
{
    private async Task<Payroll> SeedApprovedPayroll(AppDbContext context)
    {
        var dept = new Department(Guid.NewGuid(), "Engineering");
        context.Departments.Add(dept);
        var emp = new Employee(Guid.NewGuid(), "Ali", "Khan", "ali@test.com", dept.Id);
        context.Employees.Add(emp);
        var payroll = new Payroll(Guid.NewGuid(), emp.Id, 1, 2026, 40000, 5000, 1000);
        payroll.Approve();
        context.Payrolls.Add(payroll);
        await context.SaveChangesAsync();
        return payroll;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPayslip()
    {
        await using var context = CreateContext();
        var payroll = await SeedApprovedPayroll(context);
        var repo = new PayslipRepository(context);
        var payslip = new Payslip(Guid.NewGuid(), payroll.Id);

        await repo.AddAsync(payslip);

        var result = await repo.GetByIdAsync(payslip.Id);
        Assert.NotNull(result);
        Assert.Equal(payroll.Id, result.PayrollId);
        Assert.NotEqual(default(DateTime), result.GeneratedAt);
    }

    [Fact]
    public async Task GetByPayrollIdAsync_ShouldReturnMatchingPayslip()
    {
        await using var context = CreateContext();
        var payroll = await SeedApprovedPayroll(context);
        var repo = new PayslipRepository(context);
        var payslip = new Payslip(Guid.NewGuid(), payroll.Id);
        await repo.AddAsync(payslip);

        var result = await repo.GetByPayrollIdAsync(payroll.Id);

        Assert.NotNull(result);
        Assert.Equal(payslip.Id, result.Id);
    }

    [Fact]
    public async Task GetByPayrollIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new PayslipRepository(context);

        var result = await repo.GetByPayrollIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }
}
