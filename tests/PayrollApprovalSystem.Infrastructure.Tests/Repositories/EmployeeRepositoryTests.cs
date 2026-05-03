using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Persistence;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class EmployeeRepositoryTests : TestBase
{
    private async Task<Department> SeedDepartment(AppDbContext context)
    {
        var dept = new Department(Guid.NewGuid(), "Engineering");
        context.Departments.Add(dept);
        await context.SaveChangesAsync();
        return dept;
    }

    [Fact]
    public async Task AddAsync_ShouldPersistEmployee()
    {
        await using var context = CreateContext();
        var dept = await SeedDepartment(context);
        var repo = new EmployeeRepository(context);
        var employee = new Employee(Guid.NewGuid(), "Ali", "Khan", "ali@test.com", dept.Id);

        await repo.AddAsync(employee);

        var result = await repo.GetByIdAsync(employee.Id);
        Assert.NotNull(result);
        Assert.Equal("Ali", result.FirstName);
        Assert.Equal("Khan", result.LastName);
        Assert.Equal("ali@test.com", result.Email);
        Assert.True(result.IsActive);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new EmployeeRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        await using var context = CreateContext();
        var dept = await SeedDepartment(context);
        var repo = new EmployeeRepository(context);
        await repo.AddAsync(new Employee(Guid.NewGuid(), "A", "B", "a@t.com", dept.Id));
        await repo.AddAsync(new Employee(Guid.NewGuid(), "C", "D", "c@t.com", dept.Id));

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistChanges()
    {
        await using var context = CreateContext();
        var dept = await SeedDepartment(context);
        var repo = new EmployeeRepository(context);
        var employee = new Employee(Guid.NewGuid(), "Old", "Name", "old@test.com", dept.Id);
        await repo.AddAsync(employee);

        // Detach the tracked entity so UpdateAsync can attach a new instance with the same ID
        context.Entry(employee).State = EntityState.Detached;

        var updated = new Employee(employee.Id, "New", "Name", "new@test.com", dept.Id);
        updated.Deactivate();
        await repo.UpdateAsync(updated);

        var result = await repo.GetByIdAsync(employee.Id);
        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
        Assert.False(result.IsActive);
    }
}
