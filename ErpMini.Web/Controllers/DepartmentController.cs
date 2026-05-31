// ErpMini.Web/Controllers/DepartmentController.cs
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DepartmentController : BaseController
{
    private readonly IDepartmentService _service;

    public DepartmentController(
        IDepartmentService service,
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
    public async Task<IActionResult> Create(string name, string? description)
    {
        var companyId = await GetCompanyIdAsync();
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Department name is required.";
            return RedirectToAction(nameof(Index));
        }
        await _service.CreateAsync(name, description, companyId);
        TempData["Success"] = "Department added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, string? description)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.UpdateAsync(id, name, description, companyId);
        TempData["Success"] = "Department updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.DeleteAsync(id, companyId);
        TempData["Success"] = "Department deleted.";
        return RedirectToAction(nameof(Index));
    }
}