// ErpMini.Web/Controllers/UserManagementController.cs
using ErpMini.Infrastructure.Identity;
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin")]
public class UserManagementController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserContext _userContext;

    public UserManagementController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        UserContext userContext)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userContext  = userContext;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await _userContext.GetCompanyIdAsync();

        // Only show users from same company
        var users = await _userManager.Users
            .Where(u => u.CompanyId == companyId)
            .OrderBy(u => u.FullName)
            .ToListAsync();

        var userRoles = new List<(ApplicationUser User, IList<string> Roles)>();
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add((user, roles));
        }

        return View(userRoles);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(string userId, string role)
    {
        var companyId = await _userContext.GetCompanyIdAsync();
        var user = await _userManager.FindByIdAsync(userId);

        // Security: only manage users in same company
        if (user is null || user.CompanyId != companyId)
            return Forbid();

        var current = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, current);
        await _userManager.AddToRoleAsync(user, role);

        user.Role = role;
        await _userManager.UpdateAsync(user);

        TempData["Success"] = $"Role updated for {user.FullName}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(string userId)
    {
        var companyId = await _userContext.GetCompanyIdAsync();
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null || user.CompanyId != companyId)
            return Forbid();

        // Admin cannot deactivate themselves
        var currentUser = await _userContext.GetCurrentUserAsync();
        if (user.Id == currentUser?.Id)
        {
            TempData["Error"] = "You cannot deactivate your own account.";
            return RedirectToAction(nameof(Index));
        }

        user.IsActive = !user.IsActive;
        await _userManager.UpdateAsync(user);

        TempData["Success"] =
            $"{user.FullName} has been {(user.IsActive ? "activated" : "deactivated")}.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveUser(string userId)
    {
        var companyId = await _userContext.GetCompanyIdAsync();
        var user = await _userManager.FindByIdAsync(userId);

        if (user is null || user.CompanyId != companyId)
            return Forbid();

        var currentUser = await _userContext.GetCurrentUserAsync();
        if (user.Id == currentUser?.Id)
        {
            TempData["Error"] = "You cannot remove your own account.";
            return RedirectToAction(nameof(Index));
        }

        // Soft deactivate instead of hard delete
        user.IsActive = false;
        user.UserName = $"removed_{user.Id}";  // free up the email
        await _userManager.UpdateAsync(user);

        TempData["Success"] = $"{user.FullName} has been removed.";
        return RedirectToAction(nameof(Index));
    }
}