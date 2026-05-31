// ErpMini.Web/Controllers/CompanyController.cs
using ErpMini.Infrastructure.Data;
using ErpMini.Infrastructure.Identity;
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CompanyController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserContext _userContext;

    public CompanyController(AppDbContext context, UserContext userContext)
    {
        _context     = context;
        _userContext = userContext;
    }

    public async Task<IActionResult> Settings()
    {
        var companyId = await _userContext.GetCompanyIdAsync();
        if (companyId is null) return RedirectToAction("Login", "Account");

        var company = await _context.Companies
            .Include(c => c.Employees)
            .Include(c => c.Departments)
            .FirstOrDefaultAsync(c => c.Id == companyId);

        if (company is null) return NotFound();
        return View(company);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateCode(int companyId, string newCode)
    {
        newCode = newCode.Trim().ToUpper();

        // Must be admin of this company
        var myCompanyId = await _userContext.GetCompanyIdAsync();
        if (myCompanyId != companyId)
            return Forbid();

        // Check uniqueness
        var taken = await _context.Companies
            .AnyAsync(c => c.CompanyCode == newCode && c.Id != companyId);

        if (taken)
        {
            TempData["Error"] = "That code is already taken. Try another.";
            return RedirectToAction(nameof(Settings));
        }

        var company = await _context.Companies.FindAsync(companyId);
        if (company is null) return NotFound();

        company.CompanyCode = newCode;
        company.UpdatedAt   = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        TempData["Success"] = $"Company code updated to {newCode}.";
        return RedirectToAction(nameof(Settings));
    }
}