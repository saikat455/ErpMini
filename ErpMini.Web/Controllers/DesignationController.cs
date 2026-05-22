// ErpMini.Web/Controllers/DesignationController.cs
using ErpMini.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DesignationController : Controller
{
    private readonly IDesignationService _service;
    public DesignationController(IDesignationService service) => _service = service;

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(string title, string? description)
    {
        if (string.IsNullOrWhiteSpace(title)) { TempData["Error"] = "Title is required."; return RedirectToAction(nameof(Index)); }
        await _service.CreateAsync(title, description);
        TempData["Success"] = "Designation added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string title, string? description)
    {
        await _service.UpdateAsync(id, title, description);
        TempData["Success"] = "Designation updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Designation deleted.";
        return RedirectToAction(nameof(Index));
    }
}