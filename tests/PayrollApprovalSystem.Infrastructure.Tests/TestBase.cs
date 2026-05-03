using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Infrastructure.Persistence;

namespace PayrollApprovalSystem.Infrastructure.Tests;

public abstract class TestBase
{
    protected AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }
}
