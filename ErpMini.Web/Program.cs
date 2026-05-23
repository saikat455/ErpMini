// ErpMini.Web/Program.cs
using ErpMini.Application.Interfaces;
using ErpMini.Infrastructure;
using ErpMini.Infrastructure.Data;
using ErpMini.Infrastructure.Identity;
using ErpMini.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

// Must be called before any Npgsql initialization
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/Login";
    });

    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IEmployeeService, EmployeeService>();
    builder.Services.AddScoped<IDepartmentService, DepartmentService>();
    builder.Services.AddScoped<IDesignationService, DesignationService>();
    builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
    builder.Services.AddControllersWithViews();

    var app = builder.Build();

    app.UseStaticFiles();
    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine("\n=== STARTUP CRASH ===");
    Console.WriteLine(ex.GetType().Name + ": " + ex.Message);
    if (ex.InnerException != null)
        Console.WriteLine("Inner: " + ex.InnerException.Message);
    Console.WriteLine("\nPress Enter to exit...");
    Console.ReadLine();
}