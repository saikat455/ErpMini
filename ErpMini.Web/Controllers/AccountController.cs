// ErpMini.Web/Controllers/AccountController.cs
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Data;
using ErpMini.Infrastructure.Identity;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Web.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly AppDbContext _context;

    public AccountController(
        SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager,
        AppDbContext context)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _context = context;
    }

    // ── Login ──────────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(
        string email, string password,
        bool rememberMe = false, string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            ViewBag.Error = "Email and password are required.";
            return View();
        }

        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null)
        {
            ViewBag.Error = "Invalid email or password.";
            return View();
        }

        if (!user.IsActive)
        {
            ViewBag.Error = "Your account has been deactivated. Contact your admin.";
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(
            user.UserName!, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);
            return RedirectToHome(user);
        }

        ViewBag.Error = result.IsLockedOut
            ? "Account locked. Try again in 5 minutes."
            : "Invalid email or password.";

        return View();
    }

    // ── Logout ─────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    // ── Register ───────────────────────────────────────────────────
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToHome();
        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        // Check email already taken
        if (await _userManager.FindByEmailAsync(vm.Email.Trim()) is not null)
        {
            ViewBag.Error = "An account with this email already exists.";
            return View(vm);
        }

        if (vm.AccountType == "Admin")
        {
            if (string.IsNullOrWhiteSpace(vm.CompanyName))
            {
                ViewBag.Error = "Company name is required for Admin registration.";
                return View(vm);
            }

            // Create company
            var code = GenerateCompanyCode(vm.CompanyName);
            var company = new Company
            {
                Name = vm.CompanyName.Trim(),
                CompanyCode = code,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            await _context.Companies.AddAsync(company);
            await _context.SaveChangesAsync();
            await SeedCompanyDefaultsAsync(company.Id);

            // Create admin user
            var user = new ApplicationUser
            {
                FullName = vm.FullName.Trim(),
                UserName = vm.Email.Trim(),
                Email = vm.Email.Trim(),
                EmailConfirmed = true,
                IsActive = true,
                CompanyId = company.Id,
                Role = "Admin",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                await _signInManager.SignInAsync(user, isPersistent: false);
                TempData["Success"] =
                    $"Welcome! Your company code is: {code} — share it with your employees.";
                return RedirectToHome(user);
            }

            // Rollback company if user creation failed
            _context.Companies.Remove(company);
            await _context.SaveChangesAsync();

            ViewBag.Error = string.Join(" ",
                result.Errors.Select(e => e.Description));
        }
        else
        {
            // Employee registration
            if (string.IsNullOrWhiteSpace(vm.CompanyCode))
            {
                ViewBag.Error = "Company code is required.";
                return View(vm);
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c =>
                    c.CompanyCode == vm.CompanyCode.Trim().ToUpper()
                    && c.IsActive && !c.IsDeleted);

            if (company is null)
            {
                ViewBag.Error =
                    "Company code not found. Ask your admin for the correct code.";
                return View(vm);
            }

            var user = new ApplicationUser
            {
                FullName = vm.FullName.Trim(),
                UserName = vm.Email.Trim(),
                Email = vm.Email.Trim(),
                EmailConfirmed = true,
                IsActive = true,
                CompanyId = company.Id,
                Role = "Employee",
                CreatedAt = DateTime.UtcNow
            };

            var result = await _userManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");

                // Auto-create Employee record so self-service pages work
                var defaultDept = await _context.Departments
                    .FirstOrDefaultAsync(d => d.CompanyId == company.Id && d.Name == "General");
                var defaultDesig = await _context.Designations
                    .FirstOrDefaultAsync(d => d.CompanyId == company.Id && d.Title == "Employee");

                if (defaultDept is not null && defaultDesig is not null)
                {
                    var nameParts = (vm.FullName ?? vm.Email).Trim().Split(' ', 2);
                    var empCount = await _context.Employees
                        .IgnoreQueryFilters()
                        .CountAsync(e => e.CompanyId == company.Id);
                    var employee = new Employee
                    {
                        EmployeeCode = $"EMP-{(empCount + 1):D3}",
                        FirstName = nameParts[0],
                        LastName = nameParts.Length > 1 ? nameParts[1] : "",
                        Email = vm.Email.Trim(),
                        Phone = "",
                        Address = null,
                        DateOfBirth = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                        JoiningDate = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc),
                        BasicSalary = 0,
                        Gender = "",
                        DepartmentId = defaultDept.Id,
                        DesignationId = defaultDesig.Id,
                        CompanyId = company.Id,
                        UserId = user.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };
                    _context.Employees.Add(employee);
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToHome(user);
            }

            ViewBag.Error = string.Join(" ",
                result.Errors.Select(e => e.Description));
        }

        return View(vm);
    }

    // ── Access Denied ──────────────────────────────────────────────
    [HttpGet]
    public IActionResult AccessDenied() => View();

    // ── Change Password ────────────────────────────────────────────
    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword() => View();

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(
        string currentPassword, string newPassword, string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            ViewBag.Error = "Passwords do not match.";
            return View();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user is null) return RedirectToAction("Login");

        var result = await _userManager
            .ChangePasswordAsync(user, currentPassword, newPassword);

        if (result.Succeeded)
        {
            await _signInManager.RefreshSignInAsync(user);
            TempData["Success"] = "Password changed successfully.";
            return RedirectToHome(user);
        }

        ViewBag.Error = string.Join(" ",
            result.Errors.Select(e => e.Description));
        return View();
    }

    // ── Helpers ────────────────────────────────────────────────────

    private IActionResult RedirectToHome()
    {
        return User.IsInRole("Admin")
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("MyProfile", "Employee");
    }

    private IActionResult RedirectToHome(ApplicationUser user)
    {
        return user.Role == "Admin"
            ? RedirectToAction("Index", "Dashboard")
            : RedirectToAction("MyProfile", "Employee");
    }

    private static string GenerateCompanyCode(string companyName)
    {
        var prefix = new string(companyName
            .ToUpper()
            .Where(char.IsLetterOrDigit)
            .Take(4)
            .ToArray());
        var suffix = new Random().Next(1000, 9999).ToString();
        return $"{prefix}{suffix}";
    }

    private async Task SeedCompanyDefaultsAsync(int companyId)
    {
        // Default department & designation
        var dept = new Department { Name = "General", IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow };
        var desig = new Designation { Title = "Employee", IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow };
        _context.Departments.Add(dept);
        _context.Designations.Add(desig);

        // Default leave types for this company
        var leaveTypes = new List<LeaveType>
    {
        new() { Name = "Annual Leave",  TotalDays = 20, IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Sick Leave",    TotalDays = 14, IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Casual Leave",  TotalDays = 10, IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow }
    };

        // Default account categories for this company
        var categories = new List<AccountCategory>
    {
        new() { Name = "Sales Revenue",  Type = "Income",  IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Service Income", Type = "Income",  IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Office Rent",    Type = "Expense", IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Utilities",      Type = "Expense", IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow },
        new() { Name = "Salaries",       Type = "Expense", IsActive = true, CompanyId = companyId, CreatedAt = DateTime.UtcNow }
    };

        await _context.LeaveTypes.AddRangeAsync(leaveTypes);
        await _context.AccountCategories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();
    }
}