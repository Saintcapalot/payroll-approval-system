using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Persistence;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class PayrollStructureRepositoryTests : TestBase
{
    private async Task<Employee> SeedEmployee(AppDbContext context)
    {
        var dept = new Department(Guid.NewGuid(), "Engineering");
        context.Departments.Add(dept);
        var emp = new Employee(Guid.NewGuid(), "Ali", "Khan", "ali@test.com", dept.Id);
        context.Employees.Add(emp);
        await context.SaveChangesAsync();
        return emp;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistPayrollStructure()
    {
        await using var context = CreateContext();
        var emp = await SeedEmployee(context);
        var repo = new PayrollStructureRepository(context);
        var structure = new PayrollStructure(Guid.NewGuid(), emp.Id, 50000, 5000, 2000);

        await repo.AddAsync(structure);

        var result = await repo.GetActiveByEmployeeIdAsync(emp.Id);
        Assert.NotNull(result);
        Assert.Equal(50000, result.BaseSalary);
        Assert.Equal(5000, result.Bonus);
        Assert.Equal(2000, result.Deductions);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetActiveByEmployeeIdAsync_ShouldReturnNull_WhenNoActiveStructure()
    {
        await using var context = CreateContext();
        var emp = await SeedEmployee(context);
        var repo = new PayrollStructureRepository(context);

        var result = await repo.GetActiveByEmployeeIdAsync(emp.Id);

        Assert.Null(result);
    }

    [Fact]
    public async Task UpdateAsync_Deactivate_ShouldMakeStructureInactive()
    {
        await using var context = CreateContext();
        var emp = await SeedEmployee(context);
        var repo = new PayrollStructureRepository(context);
        var structure = new PayrollStructure(Guid.NewGuid(), emp.Id, 50000, 5000, 2000);
        await repo.AddAsync(structure);

        structure.Deactivate();
        await repo.UpdateAsync(structure);

        var result = await repo.GetActiveByEmployeeIdAsync(emp.Id);
        Assert.Null(result);
    }
}
