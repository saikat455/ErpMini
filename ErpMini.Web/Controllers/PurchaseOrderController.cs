// ErpMini.Web/Controllers/PurchaseOrderController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class PurchaseOrderController : Controller
{
    private readonly IProcurementService _service;
    public PurchaseOrderController(IProcurementService service) => _service = service;

    public async Task<IActionResult> Index()
        => View(await _service.GetAllOrdersAsync());

    public async Task<IActionResult> Details(int id)
    {
        var order = await _service.GetOrderByIdAsync(id);
        if (order is null) return NotFound();
        return View(order);
    }

    public async Task<IActionResult> Create()
    {
        var vendors = await _service.GetAllVendorsAsync();
        var vm = new CreatePurchaseOrderViewModel
        {
            Vendors = vendors.Select(v =>
                new SelectListItem(v.Name, v.Id.ToString())).ToList()
        };
        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePurchaseOrderViewModel vm,
        List<string> itemNames, List<string?> itemDescriptions,
        List<int> quantities, List<decimal> unitPrices)
    {
        if (!itemNames.Any() || itemNames.All(string.IsNullOrWhiteSpace))
        {
            ModelState.AddModelError("", "At least one item is required.");
        }

        if (!ModelState.IsValid)
        {
            var vendors = await _service.GetAllVendorsAsync();
            vm.Vendors = vendors.Select(v =>
                new SelectListItem(v.Name, v.Id.ToString())).ToList();
            return View(vm);
        }

        var items = itemNames
            .Select((name, i) => new CreatePurchaseOrderItemDto
            {
                ItemName = name,
                Description = i < itemDescriptions.Count ? itemDescriptions[i] : null,
                Quantity = i < quantities.Count ? quantities[i] : 1,
                UnitPrice = i < unitPrices.Count ? unitPrices[i] : 0
            })
            .Where(i => !string.IsNullOrWhiteSpace(i.ItemName))
            .ToList();

        var dto = new CreatePurchaseOrderDto
        {
            VendorId = vm.VendorId,
            OrderDate = vm.OrderDate,
            ExpectedDate = vm.ExpectedDate,
            Notes = vm.Notes,
            CreatedByUser = User.Identity?.Name ?? "Admin",
            Items = items
        };

        var success = await _service.CreateOrderAsync(dto);
        if (success)
        {
            TempData["Success"] = "Purchase order created.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to create order.");
        return View(vm);
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(int id)
    {
        await _service.SubmitOrderAsync(id);
        TempData["Success"] = "Order submitted for approval.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _service.ApproveOrderAsync(id, User.Identity?.Name ?? "Admin");
        TempData["Success"] = "Order approved.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Receive(int id)
    {
        await _service.ReceiveOrderAsync(id);
        TempData["Success"] = "Items marked as received.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int id)
    {
        await _service.CancelOrderAsync(id);
        TempData["Success"] = "Order cancelled.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteOrderAsync(id);
        TempData["Success"] = "Order deleted.";
        return RedirectToAction(nameof(Index));
    }
}