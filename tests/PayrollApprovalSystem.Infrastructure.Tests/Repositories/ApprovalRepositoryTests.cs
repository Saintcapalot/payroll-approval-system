using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Persistence;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class ApprovalRepositoryTests : TestBase
{
    private async Task<Payroll> SeedPayroll(AppDbContext context)
    {
        var dept = new Department(Guid.NewGuid(), "Engineering");
        context.Departments.Add(dept);
        var emp = new Employee(Guid.NewGuid(), "Ali", "Khan", "ali@test.com", dept.Id);
        context.Employees.Add(emp);
        var payroll = new Payroll(Guid.NewGuid(), emp.Id, 1, 2026, 40000, 5000, 1000);
        context.Payrolls.Add(payroll);
        await context.SaveChangesAsync();
        return payroll;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistApproval()
    {
        await using var context = CreateContext();
        var payroll = await SeedPayroll(context);
        var repo = new ApprovalRepository(context);
        var approval = new Approval(Guid.NewGuid(), payroll.Id);

        await repo.AddAsync(approval);

        var result = await repo.GetByIdAsync(approval.Id);
        Assert.NotNull(result);
        Assert.Equal(payroll.Id, result.PayrollId);
        Assert.Equal(Domain.Enums.ApprovalStatus.Pending, result.Status);
    }

    [Fact]
    public async Task GetByPayrollIdAsync_ShouldReturnMatchingApproval()
    {
        await using var context = CreateContext();
        var payroll = await SeedPayroll(context);
        var repo = new ApprovalRepository(context);
        var approval = new Approval(Guid.NewGuid(), payroll.Id);
        await repo.AddAsync(approval);

        var result = await repo.GetByPayrollIdAsync(payroll.Id);

        Assert.NotNull(result);
        Assert.Equal(approval.Id, result.Id);
    }

    [Fact]
    public async Task GetByPayrollIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new ApprovalRepository(context);

        var result = await repo.GetByPayrollIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Approve_ShouldPersistApprovalStatus()
    {
        await using var context = CreateContext();
        var payroll = await SeedPayroll(context);
        var repo = new ApprovalRepository(context);
        var approval = new Approval(Guid.NewGuid(), payroll.Id);
        await repo.AddAsync(approval);

        approval.Approve();
        await repo.UpdateAsync(approval);

        var result = await repo.GetByIdAsync(approval.Id);
        Assert.NotNull(result);
        Assert.Equal(Domain.Enums.ApprovalStatus.Approved, result.Status);
        Assert.NotNull(result.ReviewedAt);
    }
}
