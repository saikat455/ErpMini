// ErpMini.Web/Controllers/DesignationController.cs
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DesignationController : BaseController
{
    private readonly IDesignationService _service;

    public DesignationController(
        IDesignationService service,
        UserContext userContext) : base(userContext)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _service.GetAllAsync(companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string title, string? description)
    {
        var companyId = await GetCompanyIdAsync();
        if (string.IsNullOrWhiteSpace(title))
        {
            TempData["Error"] = "Title is required.";
            return RedirectToAction(nameof(Index));
        }
        await _service.CreateAsync(title, description, companyId);
        TempData["Success"] = "Designation added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string title, string? description)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.UpdateAsync(id, title, description, companyId);
        TempData["Success"] = "Designation updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.DeleteAsync(id, companyId);
        TempData["Success"] = "Designation deleted.";
        return RedirectToAction(nameof(Index));
    }
}