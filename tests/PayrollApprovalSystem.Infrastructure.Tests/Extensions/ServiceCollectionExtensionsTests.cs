using Microsoft.Extensions.DependencyInjection;
using PayrollApprovalSystem.Domain.Interfaces;
using PayrollApprovalSystem.Infrastructure.Extensions;

namespace PayrollApprovalSystem.Infrastructure.Tests.Extensions;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddInfrastructure_ShouldRegisterAllRepositories()
    {
        var services = new ServiceCollection();
        services.AddLogging();

        services.AddInfrastructure("Host=localhost;Database=test;Username=test;Password=test");

        var provider = services.BuildServiceProvider();

        Assert.NotNull(provider.GetService<IEmployeeRepository>());
        Assert.NotNull(provider.GetService<IDepartmentRepository>());
        Assert.NotNull(provider.GetService<IPayrollRepository>());
        Assert.NotNull(provider.GetService<IPayrollStructureRepository>());
        Assert.NotNull(provider.GetService<IApprovalRepository>());
        Assert.NotNull(provider.GetService<IPayslipRepository>());
    }
}
