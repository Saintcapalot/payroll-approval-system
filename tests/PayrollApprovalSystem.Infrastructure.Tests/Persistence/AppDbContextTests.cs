using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Tests.Persistence;

public class AppDbContextTests : TestBase
{
    [Fact]
    public void DbContext_ShouldHaveDepartmentsDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Departments);
    }

    [Fact]
    public void DbContext_ShouldHaveEmployeesDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Employees);
    }

    [Fact]
    public void DbContext_ShouldHavePayrollStructuresDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.PayrollStructures);
    }

    [Fact]
    public void DbContext_ShouldHavePayrollsDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Payrolls);
    }

    [Fact]
    public void DbContext_ShouldHaveApprovalsDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Approvals);
    }

    [Fact]
    public void DbContext_ShouldHavePayslipsDbSet()
    {
        using var context = CreateContext();
        Assert.NotNull(context.Payslips);
    }

    [Fact]
    public async Task Employee_ShouldHaveUniqueEmailIndex()
    {
        await using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Domain.Entities.Employee));
        Assert.NotNull(entityType);

        var emailIndex = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Any(p => p.Name == "Email"));
        Assert.NotNull(emailIndex);
        Assert.True(emailIndex.IsUnique);
    }

    [Fact]
    public async Task Payroll_ShouldHaveUniqueCompositeIndex()
    {
        await using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Domain.Entities.Payroll));
        Assert.NotNull(entityType);

        var compositeIndex = entityType.GetIndexes()
            .FirstOrDefault(i => i.Properties.Count == 3
                && i.Properties.Any(p => p.Name == "EmployeeId")
                && i.Properties.Any(p => p.Name == "Month")
                && i.Properties.Any(p => p.Name == "Year"));
        Assert.NotNull(compositeIndex);
        Assert.True(compositeIndex.IsUnique);
    }

    [Fact]
    public async Task Employee_FK_To_Department_ShouldUseRestrictDelete()
    {
        await using var context = CreateContext();
        var entityType = context.Model.FindEntityType(typeof(Domain.Entities.Employee));
        Assert.NotNull(entityType);

        var fk = entityType.GetForeignKeys().FirstOrDefault();
        Assert.NotNull(fk);
        Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
    }
}
