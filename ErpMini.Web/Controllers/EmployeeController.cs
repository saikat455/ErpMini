// ErpMini.Web/Controllers/EmployeeController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class EmployeeController : BaseController
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;

    public EmployeeController(
        IEmployeeService emp,
        IDepartmentService dept,
        IDesignationService desig,
        UserContext userContext) : base(userContext)
    {
        _employeeService = emp;
        _departmentService = dept;
        _designationService = desig;
    }

    public async Task<IActionResult> Index(int page = 1, string? search = null)
    {
        var companyId = await GetCompanyIdAsync();
        var employees = await _employeeService.GetAllAsync(companyId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.ToLower();
            employees = employees.Where(e =>
                e.FullName.ToLower().Contains(search) ||
                e.EmployeeCode.ToLower().Contains(search) ||
                e.DepartmentName.ToLower().Contains(search) ||
                e.Email.ToLower().Contains(search));
        }

        ViewBag.Search = search;
        return View(employees);
    }

    public async Task<IActionResult> Create()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await BuildViewModel(new EmployeeViewModel
        {
            JoiningDate = DateTime.Today,
            DateOfBirth = DateTime.Today.AddYears(-25)
        }, companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();

        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, companyId));

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
            DesignationId = vm.DesignationId,
            CompanyId = companyId
        };

        var success = await _employeeService.CreateAsync(dto);
        if (success)
        {
            TempData["Success"] = "Employee created successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to create employee.");
        return View(await BuildViewModel(vm, companyId));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var companyId = await GetCompanyIdAsync();
        var emp = await _employeeService.GetByIdAsync(id, companyId);
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
        }, companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, EmployeeViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();

        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, companyId));

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
            DesignationId = vm.DesignationId,
            CompanyId = companyId
        };

        var success = await _employeeService.UpdateAsync(id, dto);
        if (success)
        {
            TempData["Success"] = "Employee updated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to update employee.");
        return View(await BuildViewModel(vm, companyId));
    }

    public async Task<IActionResult> Details(int id)
    {
        var companyId = await GetCompanyIdAsync();
        var emp = await _employeeService.GetByIdAsync(id, companyId);
        if (emp is null) return NotFound();
        return View(emp);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _employeeService.DeleteAsync(id, companyId);
        TempData["Success"] = "Employee deleted.";
        return RedirectToAction(nameof(Index));
    }

    private async Task<EmployeeViewModel> BuildViewModel(
        EmployeeViewModel vm, int companyId)
    {
        var depts = await _departmentService.GetAllAsync(companyId);
        var desigs = await _designationService.GetAllAsync(companyId);

        vm.Departments = depts.Select(d =>
            new SelectListItem(d.Name, d.Id.ToString())).ToList();
        vm.Designations = desigs.Select(d =>
            new SelectListItem(d.Title, d.Id.ToString())).ToList();
        return vm;
    }
}