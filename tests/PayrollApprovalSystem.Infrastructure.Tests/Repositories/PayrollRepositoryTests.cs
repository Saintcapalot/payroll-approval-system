using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Persistence;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class PayrollRepositoryTests : TestBase
{
    private async Task<(Department dept, Employee emp)> SeedEmployee(AppDbContext context)
    {
        var dept = new Department(Guid.NewGuid(), "Engineering");
        context.Departments.Add(dept);
        var emp = new Employee(Guid.NewGuid(), "Ali", "Khan", "ali@test.com", dept.Id);
        context.Employees.Add(emp);
        await context.SaveChangesAsync();
        return (dept, emp);
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPayroll()
    {
        await using var context = CreateContext();
        var (_, emp) = await SeedEmployee(context);
        var repo = new PayrollRepository(context);
        var payroll = new Payroll(Guid.NewGuid(), emp.Id, 1, 2026, 40000, 5000, 1000);

        await repo.AddAsync(payroll);

        var result = await repo.GetByIdAsync(payroll.Id);
        Assert.NotNull(result);
        Assert.Equal(emp.Id, result.EmployeeId);
        Assert.Equal(44000, result.TotalAmount);
    }

    [Fact]
    public async Task ExistsForMonthAsync_ShouldReturnTrue_WhenPayrollExists()
    {
        await using var context = CreateContext();
        var (_, emp) = await SeedEmployee(context);
        var repo = new PayrollRepository(context);
        await repo.AddAsync(new Payroll(Guid.NewGuid(), emp.Id, 3, 2026, 40000, 0, 0));

        var exists = await repo.ExistsForMonthAsync(emp.Id, 3, 2026);

        Assert.True(exists);
    }

    [Fact]
    public async Task ExistsForMonthAsync_ShouldReturnFalse_WhenNoPayroll()
    {
        await using var context = CreateContext();
        var (_, emp) = await SeedEmployee(context);
        var repo = new PayrollRepository(context);

        var exists = await repo.ExistsForMonthAsync(emp.Id, 3, 2026);

        Assert.False(exists);
    }

    [Fact]
    public async Task GetByEmployeeIdAsync_ShouldReturnOnlyMatchingPayrolls()
    {
        await using var context = CreateContext();
        var (_, emp) = await SeedEmployee(context);
        var repo = new PayrollRepository(context);
        await repo.AddAsync(new Payroll(Guid.NewGuid(), emp.Id, 1, 2026, 40000, 0, 0));
        await repo.AddAsync(new Payroll(Guid.NewGuid(), emp.Id, 2, 2026, 45000, 0, 0));

        var result = await repo.GetByEmployeeIdAsync(emp.Id);

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistStatusChange()
    {
        await using var context = CreateContext();
        var (_, emp) = await SeedEmployee(context);
        var repo = new PayrollRepository(context);
        var payroll = new Payroll(Guid.NewGuid(), emp.Id, 1, 2026, 40000, 0, 0);
        await repo.AddAsync(payroll);

        payroll.Approve();
        await repo.UpdateAsync(payroll);

        var result = await repo.GetByIdAsync(payroll.Id);
        Assert.NotNull(result);
        Assert.Equal(Domain.Enums.PayrollStatus.Approved, result.Status);
    }
}
