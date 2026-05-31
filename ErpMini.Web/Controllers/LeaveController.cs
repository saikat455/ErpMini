// ErpMini.Web/Controllers/LeaveController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class LeaveController : Controller
{
    private readonly ILeaveService _leaveService;
    private readonly IEmployeeService _employeeService;

    public LeaveController(ILeaveService leaveService, IEmployeeService employeeService)
    {
        _leaveService = leaveService;
        _employeeService = employeeService;
    }

    // Admin: all applications
    public async Task<IActionResult> Index()
    {
        var leaves = await _leaveService.GetAllAsync();
        return View(leaves);
    }

   [Authorize(Roles = "Admin,HR")]
    public async Task<IActionResult> Pending()
    {
        var leaves = await _leaveService.GetPendingAsync();
        return View(leaves);
    }

    // Apply leave form
    public async Task<IActionResult> Apply()
    {
        return View(await BuildViewModel(new ApplyLeaveViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyLeaveViewModel vm)
    {
        if (vm.ToDate < vm.FromDate)
            ModelState.AddModelError("ToDate", "To date cannot be before from date.");

        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm));

        var dto = new CreateLeaveDto
        {
            EmployeeId = vm.EmployeeId,
            LeaveTypeId = vm.LeaveTypeId,
            FromDate = vm.FromDate,
            ToDate = vm.ToDate,
            Reason = vm.Reason
        };

        var success = await _leaveService.ApplyAsync(dto);
        if (success)
        {
            TempData["Success"] = "Leave application submitted successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to submit leave application.");
        return View(await BuildViewModel(vm));
    }

    [HttpPost]
     [Authorize(Roles = "Admin,HR")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var userName = User.Identity?.Name ?? "Admin";
        await _leaveService.ApproveAsync(id, userName);
        TempData["Success"] = "Leave approved.";
        return RedirectToAction(nameof(Pending));
    }

    [HttpPost]
    [Authorize(Roles = "Admin,HR")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(RejectLeaveViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToAction(nameof(Pending));
        }

        var userName = User.Identity?.Name ?? "Admin";
        await _leaveService.RejectAsync(vm.LeaveId, vm.Reason, userName);
        TempData["Success"] = "Leave rejected.";
        return RedirectToAction(nameof(Pending));
    }

    // Employee leave balance view
    public async Task<IActionResult> Balance(int employeeId)
    {
        var balance = await _leaveService.GetBalanceAsync(employeeId);
        var employees = await _employeeService.GetAllAsync();
        ViewBag.Employees = new SelectList(employees, "Id", "FullName", employeeId);
        ViewBag.SelectedEmployeeId = employeeId;
        return View(balance);
    }

    // Leave type management
    public async Task<IActionResult> LeaveTypes()
    {
        var types = await _leaveService.GetLeaveTypesAsync();
        return View(types);
    }

    [HttpPost]
    public async Task<IActionResult> CreateLeaveType(string name, int totalDays, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Leave type name is required.";
            return RedirectToAction(nameof(LeaveTypes));
        }
        await _leaveService.CreateLeaveTypeAsync(name, totalDays, description);
        TempData["Success"] = "Leave type added.";
        return RedirectToAction(nameof(LeaveTypes));
    }

    [HttpPost]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        await _leaveService.DeleteLeaveTypeAsync(id);
        TempData["Success"] = "Leave type deleted.";
        return RedirectToAction(nameof(LeaveTypes));
    }

    private async Task<ApplyLeaveViewModel> BuildViewModel(ApplyLeaveViewModel vm)
    {
        var employees = await _employeeService.GetAllAsync();
        var leaveTypes = await _leaveService.GetLeaveTypesAsync();

        vm.Employees = employees
            .Select(e => new SelectListItem($"{e.FullName} ({e.EmployeeCode})", e.Id.ToString()))
            .ToList();
        vm.LeaveTypes = leaveTypes
            .Select(lt => new SelectListItem($"{lt.Name} ({lt.TotalDays} days)", lt.Id.ToString()))
            .ToList();
        return vm;
    }
}