using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;

namespace PayrollApprovalSystem.Infrastructure.Persistence;

public static class DatabaseSeeder
{
    private static readonly Guid DemoDepartmentId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid DemoEmployeeId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid DemoPayrollStructureId = Guid.Parse("33333333-3333-3333-3333-333333333333");

    public static async Task SeedDevelopmentDataAsync(AppDbContext dbContext)
    {
        var department = await dbContext.Departments
            .FirstOrDefaultAsync(d => d.Id == DemoDepartmentId || d.Name == "Engineering");

        if (department is null)
        {
            department = new Department(DemoDepartmentId, "Engineering");
            await dbContext.Departments.AddAsync(department);
            await dbContext.SaveChangesAsync();
        }

        var employee = await dbContext.Employees
            .FirstOrDefaultAsync(e => e.Id == DemoEmployeeId || e.Email == "demo.employee@payroll.local");

        if (employee is null)
        {
            employee = new Employee(
                DemoEmployeeId,
                "Demo",
                "Employee",
                "demo.employee@payroll.local",
                department.Id);

            await dbContext.Employees.AddAsync(employee);
            await dbContext.SaveChangesAsync();
        }

        var activePayrollStructure = await dbContext.PayrollStructures
            .FirstOrDefaultAsync(ps => ps.EmployeeId == employee.Id && ps.IsActive);

        if (activePayrollStructure is null)
        {
            activePayrollStructure = new PayrollStructure(
                DemoPayrollStructureId,
                employee.Id,
                50000m,
                5000m,
                1500m);

            await dbContext.PayrollStructures.AddAsync(activePayrollStructure);
            await dbContext.SaveChangesAsync();
        }
    }
}
