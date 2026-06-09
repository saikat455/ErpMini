// ErpMini.Web/Controllers/VendorController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin")]
public class VendorController : BaseController
{
    private readonly IProcurementService _service;

    public VendorController(
        IProcurementService service,
        UserContext userContext) : base(userContext)
    {
        _service = service;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _service.GetAllVendorsAsync(companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(VendorViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Please fill all required fields.";
            return RedirectToAction(nameof(Index));
        }
        await _service.CreateVendorAsync(new VendorDto
        {
            Name = vm.Name,
            ContactPerson = vm.ContactPerson,
            Email = vm.Email,
            Phone = vm.Phone,
            Address = vm.Address
        }, companyId);
        TempData["Success"] = "Vendor added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(VendorViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.UpdateVendorAsync(new VendorDto
        {
            Id = vm.Id,
            Name = vm.Name,
            ContactPerson = vm.ContactPerson,
            Email = vm.Email,
            Phone = vm.Phone,
            Address = vm.Address
        }, companyId);
        TempData["Success"] = "Vendor updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.DeleteVendorAsync(id, companyId);
        TempData["Success"] = "Vendor deleted.";
        return RedirectToAction(nameof(Index));
    }
}