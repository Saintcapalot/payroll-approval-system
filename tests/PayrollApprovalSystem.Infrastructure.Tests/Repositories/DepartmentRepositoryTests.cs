using PayrollApprovalSystem.Domain.Entities;
using PayrollApprovalSystem.Infrastructure.Repositories;

namespace PayrollApprovalSystem.Infrastructure.Tests.Repositories;

public class DepartmentRepositoryTests : TestBase
{
    [Fact]
    public async Task AddAsync_ShouldPersistDepartment()
    {
        await using var context = CreateContext();
        var repo = new DepartmentRepository(context);
        var department = new Department(Guid.NewGuid(), "Engineering");

        await repo.AddAsync(department);

        var result = await repo.GetByIdAsync(department.Id);
        Assert.NotNull(result);
        Assert.Equal("Engineering", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        await using var context = CreateContext();
        var repo = new DepartmentRepository(context);

        var result = await repo.GetByIdAsync(Guid.NewGuid());

        Assert.Null(result);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllDepartments()
    {
        await using var context = CreateContext();
        var repo = new DepartmentRepository(context);
        await repo.AddAsync(new Department(Guid.NewGuid(), "HR"));
        await repo.AddAsync(new Department(Guid.NewGuid(), "Finance"));

        var result = await repo.GetAllAsync();

        Assert.Equal(2, result.Count);
    }
}
