// ErpMini.Infrastructure/Data/AppDbContext.cs
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ErpMini.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ICompanyContext _companyContext;

    public AppDbContext(
        DbContextOptions<AppDbContext> options,
        ICompanyContext companyContext) : base(options)
    {
        _companyContext = companyContext;
    }

    public DbSet<Company> Companies => Set<Company>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Department> Departments => Set<Department>();
    public DbSet<Designation> Designations => Set<Designation>();
    public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();
    public DbSet<LeaveApplication> LeaveApplications => Set<LeaveApplication>();
    public DbSet<Payroll> Payrolls => Set<Payroll>();
    public DbSet<Vendor> Vendors => Set<Vendor>();
    public DbSet<PurchaseOrder> PurchaseOrders => Set<PurchaseOrder>();
    public DbSet<PurchaseOrderItem> PurchaseOrderItems => Set<PurchaseOrderItem>();
    public DbSet<AccountCategory> AccountCategories => Set<AccountCategory>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    public override async Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        var companyId = await _companyContext.GetCompanyIdAsync();

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
            {
                entry.Entity.CreatedAt = DateTime.UtcNow;
                if (entry.Entity.CompanyId == 0 && companyId.HasValue
                    && entry.Entity is not Company)
                {
                    entry.Entity.CompanyId = companyId.Value;
                }
            }

            if (entry.State is EntityState.Modified or EntityState.Added)
            {
                entry.Entity.UpdatedAt = DateTime.UtcNow;
            }
        }

        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("erp");
        builder.Entity<ApplicationUser>().ToTable("users");

        // ── Soft delete filters ──────────────────────────────────────
        builder.Entity<Company>().HasQueryFilter(c => !c.IsDeleted);
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Designation>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<LeaveType>().HasQueryFilter(l => !l.IsDeleted);
        builder.Entity<LeaveApplication>().HasQueryFilter(l => !l.IsDeleted);
        builder.Entity<Payroll>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
        builder.Entity<PurchaseOrder>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<PurchaseOrderItem>().HasQueryFilter(p => !p.IsDeleted);
        builder.Entity<AccountCategory>().HasQueryFilter(a => !a.IsDeleted);
        builder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);

        // ── All entities → Company (CompanyId from BaseEntity) ───────
        builder.Entity<Employee>()
            .HasOne<Company>()
            .WithMany(c => c.Employees)
            .HasForeignKey(e => e.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Department>()
            .HasOne<Company>()
            .WithMany(c => c.Departments)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Designation>()
            .HasOne<Company>()
            .WithMany(c => c.Designations)
            .HasForeignKey(d => d.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Vendor>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(v => v.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrder>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrderItem>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(pi => pi.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Transaction>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(t => t.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveApplication>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveType>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(l => l.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payroll>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(p => p.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<AccountCategory>()
            .HasOne<Company>()
            .WithMany()
            .HasForeignKey(a => a.CompanyId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Employee relationships ───────────────────────────────────
        builder.Entity<Employee>()
            .HasOne(e => e.Department)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DepartmentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Employee>()
            .HasOne(e => e.Designation)
            .WithMany(d => d.Employees)
            .HasForeignKey(e => e.DesignationId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Leave ────────────────────────────────────────────────────
        builder.Entity<LeaveApplication>()
            .HasOne(l => l.Employee)
            .WithMany()
            .HasForeignKey(l => l.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<LeaveApplication>()
            .HasOne(l => l.LeaveType)
            .WithMany(lt => lt.LeaveApplications)
            .HasForeignKey(l => l.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // ── Payroll ──────────────────────────────────────────────────
        builder.Entity<Payroll>()
            .HasOne(p => p.Employee)
            .WithMany()
            .HasForeignKey(p => p.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Payroll>()
            .HasIndex(p => new { p.EmployeeId, p.Month, p.Year })
            .IsUnique();

        // ── Procurement ──────────────────────────────────────────────
        builder.Entity<PurchaseOrder>()
            .HasOne(p => p.Vendor)
            .WithMany(v => v.PurchaseOrders)
            .HasForeignKey(p => p.VendorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<PurchaseOrderItem>()
            .HasOne(i => i.PurchaseOrder)
            .WithMany(p => p.Items)
            .HasForeignKey(i => i.PurchaseOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<PurchaseOrderItem>()
            .Ignore(i => i.TotalPrice);

        // ── Accounts ─────────────────────────────────────────────────
        builder.Entity<Transaction>()
            .HasOne(t => t.Category)
            .WithMany(c => c.Transactions)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}