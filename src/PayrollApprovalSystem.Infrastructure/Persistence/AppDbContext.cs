using Microsoft.EntityFrameworkCore;
using PayrollApprovalSystem.Domain.Entities;

namespace PayrollApprovalSystem.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<PayrollStructure> PayrollStructures => Set<PayrollStructure>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<Approval> Approvals => Set<Approval>();
    public DbSet<Payslip> Payslips => Set<Payslip>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
            entity.HasIndex(e => e.Email).IsUnique();

            entity.HasOne<Department>()
                .WithMany()
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<PayrollStructure>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Bonus).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Deductions).HasColumnType("decimal(18,2)");

            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payroll>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.BaseSalary).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Bonus).HasColumnType("decimal(18,2)");
            entity.Property(e => e.Deductions).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

            entity.HasIndex(e => new { e.EmployeeId, e.Month, e.Year }).IsUnique();

            entity.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Approval>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne<Payroll>()
                .WithMany()
                .HasForeignKey(e => e.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne<Payroll>()
                .WithMany()
                .HasForeignKey(e => e.PayrollId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
