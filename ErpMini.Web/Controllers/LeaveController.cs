// ErpMini.Web/Controllers/LeaveController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class LeaveController : BaseController
{
    private readonly ILeaveService _leaveService;
    private readonly IEmployeeService _employeeService;

    public LeaveController(
        ILeaveService leaveService,
        IEmployeeService employeeService,
        UserContext userContext) : base(userContext)
    {
        _leaveService = leaveService;
        _employeeService = employeeService;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _leaveService.GetAllAsync(companyId));
    }

    public async Task<IActionResult> MyLeaves()
    {
        var companyId = await GetCompanyIdAsync();
        var employees = await _employeeService.GetAllAsync(companyId);
        var emp = employees.FirstOrDefault(e =>
            string.Equals(e.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase));

        if (emp is null) return RedirectToAction("MyProfile", "Employee");

        var leaves = await _leaveService.GetByEmployeeAsync(emp.Id, companyId);
        return View(leaves);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Pending()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _leaveService.GetPendingAsync(companyId));
    }

    public async Task<IActionResult> Apply()
    {
        var companyId = await GetCompanyIdAsync();
        var isAdmin = User.IsInRole("Admin");
        var vm = new ApplyLeaveViewModel();

        if (!isAdmin)
        {
            var employees = await _employeeService.GetAllAsync(companyId);
            var emp = employees.FirstOrDefault(e =>
                string.Equals(e.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase));
            if (emp is null) return RedirectToAction("MyProfile", "Employee");
            vm.EmployeeId = emp.Id;
        }

        return View(await BuildApplyViewModel(vm, companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(ApplyLeaveViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        var isAdmin = User.IsInRole("Admin");

        if (vm.ToDate < vm.FromDate)
            ModelState.AddModelError("ToDate", "To date cannot be before from date.");

        if (!isAdmin)
        {
            var employees = await _employeeService.GetAllAsync(companyId);
            var emp = employees.FirstOrDefault(e =>
                string.Equals(e.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase));
            if (emp is null) return RedirectToAction("MyProfile", "Employee");
            vm.EmployeeId = emp.Id;
        }

        if (!ModelState.IsValid)
            return View(await BuildApplyViewModel(vm, companyId));

        var success = await _leaveService.ApplyAsync(new CreateLeaveDto
        {
            EmployeeId = vm.EmployeeId,
            LeaveTypeId = vm.LeaveTypeId,
            FromDate = vm.FromDate,
            ToDate = vm.ToDate,
            Reason = vm.Reason,
            CompanyId = companyId
        });

        if (success)
        {
            TempData["Success"] = "Leave application submitted.";
            return RedirectToAction(isAdmin ? nameof(Index) : nameof(MyLeaves));
        }

        ModelState.AddModelError("", "Failed to submit.");
        return View(await BuildApplyViewModel(vm, companyId));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        var companyId = await GetCompanyIdAsync();
        var userName = User.Identity?.Name ?? "Admin";
        await _leaveService.ApproveAsync(id, userName, companyId);
        TempData["Success"] = "Leave approved.";
        return RedirectToAction(nameof(Pending));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(RejectLeaveViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "Rejection reason is required.";
            return RedirectToAction(nameof(Pending));
        }
        var userName = User.Identity?.Name ?? "Admin";
        await _leaveService.RejectAsync(vm.LeaveId, vm.Reason, userName, companyId);
        TempData["Success"] = "Leave rejected.";
        return RedirectToAction(nameof(Pending));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Balance(int employeeId)
    {
        var companyId = await GetCompanyIdAsync();
        var balance = await _leaveService.GetBalanceAsync(employeeId, companyId);
        var employees = await _employeeService.GetAllAsync(companyId);
        ViewBag.Employees = new SelectList(employees, "Id", "FullName", employeeId);
        ViewBag.SelectedEmployeeId = employeeId;
        return View(balance);
    }

    public async Task<IActionResult> LeaveTypes()
    {
        var types = await _leaveService.GetLeaveTypesAsync();
        return View(types);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLeaveType(
        string name, int totalDays, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Name is required.";
            return RedirectToAction(nameof(LeaveTypes));
        }
        var companyId = await GetCompanyIdAsync();
        await _leaveService.CreateLeaveTypeAsync(name, totalDays, description, companyId);
        TempData["Success"] = "Leave type added.";
        return RedirectToAction(nameof(LeaveTypes));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteLeaveType(int id)
    {
        await _leaveService.DeleteLeaveTypeAsync(id);
        TempData["Success"] = "Leave type deleted.";
        return RedirectToAction(nameof(LeaveTypes));
    }

    private async Task<ApplyLeaveViewModel> BuildApplyViewModel(
        ApplyLeaveViewModel vm, int companyId)
    {
        var isAdmin = User.IsInRole("Admin");
        var leaveTypes = await _leaveService.GetLeaveTypesAsync();

        if (isAdmin)
        {
            var employees = await _employeeService.GetAllAsync(companyId);
            vm.Employees = employees
                .Select(e => new SelectListItem(
                    $"{e.FullName} ({e.EmployeeCode})", e.Id.ToString()))
                .ToList();
        }

        vm.LeaveTypes = leaveTypes
            .Select(lt => new SelectListItem(
                $"{lt.Name} ({lt.TotalDays} days)", lt.Id.ToString()))
            .ToList();
        return vm;
    }
}
