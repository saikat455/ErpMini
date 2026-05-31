// ErpMini.Web/Controllers/EmployeeController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ErpMini.Application.Helpers;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin,HR")]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;

    public EmployeeController(IEmployeeService emp, IDepartmentService dept, IDesignationService desig)
    {
        _employeeService = emp;
        _departmentService = dept;
        _designationService = desig;
    }


public async Task<IActionResult> Index(int page = 1, string? search = null)
{
    var employees = await _employeeService.GetAllAsync();

    if (!string.IsNullOrWhiteSpace(search))
    {
        search = search.ToLower();
        employees = employees.Where(e =>
            e.FullName.ToLower().Contains(search) ||
            e.EmployeeCode.ToLower().Contains(search) ||
            e.DepartmentName.ToLower().Contains(search) ||
            e.Email.ToLower().Contains(search));
    }

    var paged = PagedList<EmployeeDto>.Create(employees, page, pageSize: 10);
    ViewBag.Search = search;
    return View(paged);
}

    public async Task<IActionResult> Create()
    {
        return View(await BuildViewModel(new EmployeeViewModel
        {
            JoiningDate = DateTime.Today,
            DateOfBirth = DateTime.Today.AddYears(-25)
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm));

        var dto = new CreateEmployeeDto
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            Email = vm.Email,
            Phone = vm.Phone,
            Address = vm.Address,
            DateOfBirth = vm.DateOfBirth,
            JoiningDate = vm.JoiningDate,
            BasicSalary = vm.BasicSalary,
            Gender = vm.Gender,
            DepartmentId = vm.DepartmentId,
            DesignationId = vm.DesignationId
        };

        var success = await _employeeService.CreateAsync(dto);
        if (success)
        {
            TempData["Success"] = "Employee created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to create employee.");
        return View(await BuildViewModel(vm));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp is null) return NotFound();

        return View(await BuildViewModel(new EmployeeViewModel
        {
            Id = emp.Id,
            FirstName = emp.FirstName,
            LastName = emp.LastName,
            Email = emp.Email,
            Phone = emp.Phone,
            Address = emp.Address,
            DateOfBirth = emp.DateOfBirth,
            JoiningDate = emp.JoiningDate,
            BasicSalary = emp.BasicSalary,
            Gender = emp.Gender,
            DepartmentId = emp.DepartmentId,
            DesignationId = emp.DesignationId
        }));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm));

        var dto = new CreateEmployeeDto
        {
            FirstName = vm.FirstName, LastName = vm.LastName,
            Email = vm.Email, Phone = vm.Phone, Address = vm.Address,
            DateOfBirth = vm.DateOfBirth, JoiningDate = vm.JoiningDate,
            BasicSalary = vm.BasicSalary, Gender = vm.Gender,
            DepartmentId = vm.DepartmentId, DesignationId = vm.DesignationId
        };

        var success = await _employeeService.UpdateAsync(id, dto);
        if (success)
        {
            TempData["Success"] = "Employee updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to update employee.");
        return View(await BuildViewModel(vm));
    }

    public async Task<IActionResult> Details(int id)
    {
        var emp = await _employeeService.GetByIdAsync(id);
        if (emp is null) return NotFound();
        return View(emp);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await _employeeService.DeleteAsync(id);
        TempData["Success"] = "Employee deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    // Helper to populate dropdowns
    private async Task<EmployeeViewModel> BuildViewModel(EmployeeViewModel vm)
    {
        var depts = await _departmentService.GetAllAsync();
        var desigs = await _designationService.GetAllAsync();

        vm.Departments = depts.Select(d => new SelectListItem(d.Name, d.Id.ToString())).ToList();
        vm.Designations = desigs.Select(d => new SelectListItem(d.Title, d.Id.ToString())).ToList();
        return vm;
    }
}