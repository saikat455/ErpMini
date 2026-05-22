// ErpMini.Web/Controllers/DepartmentController.cs
using ErpMini.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DepartmentController : Controller
{
    private readonly IDepartmentService _service;
    public DepartmentController(IDepartmentService service) => _service = service;

    public async Task<IActionResult> Index()
        => View(await _service.GetAllAsync());

    [HttpPost]
    public async Task<IActionResult> Create(string name, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Department name is required.";
            return RedirectToAction(nameof(Index));
        }
        await _service.CreateAsync(name, description);
        TempData["Success"] = "Department added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, string name, string? description)
    {
        await _service.UpdateAsync(id, name, description);
        TempData["Success"] = "Department updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Department deleted.";
        return RedirectToAction(nameof(Index));
    }
}