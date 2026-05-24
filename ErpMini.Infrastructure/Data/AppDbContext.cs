// ErpMini.Infrastructure/Data/AppDbContext.cs
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

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


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.HasDefaultSchema("erp");
        builder.Entity<ApplicationUser>().ToTable("users");

        // Soft delete global filters
        builder.Entity<Employee>().HasQueryFilter(e => !e.IsDeleted);
        builder.Entity<Department>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<Designation>().HasQueryFilter(d => !d.IsDeleted);
        builder.Entity<LeaveType>().HasQueryFilter(l => !l.IsDeleted);
builder.Entity<LeaveApplication>().HasQueryFilter(l => !l.IsDeleted);
// Add inside OnModelCreating
builder.Entity<Vendor>().HasQueryFilter(v => !v.IsDeleted);
builder.Entity<PurchaseOrder>().HasQueryFilter(p => !p.IsDeleted);
builder.Entity<PurchaseOrderItem>().HasQueryFilter(p => !p.IsDeleted);

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

// TotalPrice is computed — not stored in DB
builder.Entity<PurchaseOrderItem>()
    .Ignore(i => i.TotalPrice);
    
        // Relationships
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

builder.Entity<Payroll>().HasQueryFilter(p => !p.IsDeleted);

builder.Entity<Payroll>()
    .HasOne(p => p.Employee)
    .WithMany()
    .HasForeignKey(p => p.EmployeeId)
    .OnDelete(DeleteBehavior.Restrict);

// Unique constraint: one payroll per employee per month/year
builder.Entity<Payroll>()
    .HasIndex(p => new { p.EmployeeId, p.Month, p.Year })
    .IsUnique();

        // Seed data
builder.Entity<Department>().HasData(
    new Department { Id = 1, Name = "Human Resources", Description = "HR Department", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Department { Id = 2, Name = "Information Technology", Description = "IT Department", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Department { Id = 3, Name = "Finance", Description = "Finance Department", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
);

builder.Entity<Designation>().HasData(
    new Designation { Id = 1, Title = "Software Engineer", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Designation { Id = 2, Title = "Senior Engineer", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Designation { Id = 3, Title = "HR Manager", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new Designation { Id = 4, Title = "Accountant", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
);

// Seed leave types
builder.Entity<LeaveType>().HasData(
    new LeaveType { Id = 1, Name = "Annual Leave", TotalDays = 20, Description = "Yearly paid leave", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new LeaveType { Id = 2, Name = "Sick Leave", TotalDays = 14, Description = "Medical leave", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new LeaveType { Id = 3, Name = "Casual Leave", TotalDays = 10, Description = "Short personal leave", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
);

builder.Entity<AccountCategory>().HasQueryFilter(a => !a.IsDeleted);
builder.Entity<Transaction>().HasQueryFilter(t => !t.IsDeleted);

builder.Entity<Transaction>()
    .HasOne(t => t.Category)
    .WithMany(c => c.Transactions)
    .HasForeignKey(t => t.CategoryId)
    .OnDelete(DeleteBehavior.Restrict);

// Seed categories
builder.Entity<AccountCategory>().HasData(
    new AccountCategory { Id = 1, Name = "Sales Revenue", Type = "Income", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new AccountCategory { Id = 2, Name = "Service Income", Type = "Income", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new AccountCategory { Id = 3, Name = "Office Rent", Type = "Expense", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new AccountCategory { Id = 4, Name = "Utilities", Type = "Expense", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) },
    new AccountCategory { Id = 5, Name = "Salaries", Type = "Expense", IsActive = true, CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc) }
);
    }
}