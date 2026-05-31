// ErpMini.Web/Controllers/PayrollController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class PayrollController : BaseController
{
    private readonly IPayrollService _payrollService;
    private readonly IEmployeeService _employeeService;

    public PayrollController(
        IPayrollService payroll,
        IEmployeeService employee,
        UserContext userContext) : base(userContext)
    {
        _payrollService = payroll;
        _employeeService = employee;
    }

    public async Task<IActionResult> Index(int? month, int? year)
    {
        var companyId = await GetCompanyIdAsync();
        var selectedMonth = month ?? DateTime.Today.Month;
        var selectedYear = year ?? DateTime.Today.Year;

        var payrolls = await _payrollService
            .GetByMonthYearAsync(selectedMonth, selectedYear, companyId);

        return View(new PayrollFilterViewModel
        {
            Month = selectedMonth,
            Year = selectedYear,
            Payrolls = payrolls,
            Months = BuildMonthList(selectedMonth),
            Years = BuildYearList(selectedYear)
        });
    }

    public async Task<IActionResult> History()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _payrollService.GetAllAsync(companyId));
    }

    public async Task<IActionResult> Summary()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _payrollService.GetSummaryAsync(companyId));
    }

    public async Task<IActionResult> Payslip(int id)
    {
        var companyId = await GetCompanyIdAsync();
        var payroll = await _payrollService.GetByIdAsync(id, companyId);
        if (payroll is null) return NotFound();
        return View(payroll);
    }

    public async Task<IActionResult> Generate()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await BuildGenerateViewModel(
            new GeneratePayrollViewModel(), companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GeneratePayrollViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();

        if (!ModelState.IsValid)
            return View(await BuildGenerateViewModel(vm, companyId));

        if (await _payrollService.ExistsAsync(
                vm.EmployeeId, vm.Month, vm.Year))
        {
            ModelState.AddModelError("",
                "Payroll already exists for this employee this month.");
            return View(await BuildGenerateViewModel(vm, companyId));
        }

        var success = await _payrollService.GenerateAsync(new GeneratePayrollDto
        {
            EmployeeId = vm.EmployeeId,
            CompanyId = companyId,
            Month = vm.Month,
            Year = vm.Year,
            BasicSalary = vm.BasicSalary,
            Bonus = vm.Bonus,
            Deduction = vm.Deduction,
            Note = vm.Note,
            GeneratedBy = User.Identity?.Name ?? "Admin"
        });

        if (success)
        {
            TempData["Success"] = "Payroll generated.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to generate payroll.");
        return View(await BuildGenerateViewModel(vm, companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkGenerate(int month, int year)
    {
        var companyId = await GetCompanyIdAsync();
        var generatedBy = User.Identity?.Name ?? "Admin";
        var success = await _payrollService
            .GenerateBulkAsync(month, year, generatedBy, companyId);

        TempData[success ? "Success" : "Error"] = success
            ? $"Payroll generated for all employees — {new DateTime(year, month, 1):MMMM yyyy}."
            : "All employees already have payroll for this month.";

        return RedirectToAction(nameof(Index), new { month, year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id, int month, int year)
    {
        var companyId = await GetCompanyIdAsync();
        await _payrollService.MarkAsPaidAsync(id, companyId);
        TempData["Success"] = "Marked as paid.";
        return RedirectToAction(nameof(Index), new { month, year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _payrollService.DeleteAsync(id, companyId);
        TempData["Success"] = "Payroll deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> GetEmployeeSalary(int employeeId)
    {
        var companyId = await GetCompanyIdAsync();
        var emp = await _employeeService.GetByIdAsync(employeeId, companyId);
        return Json(new { salary = emp?.BasicSalary ?? 0 });
    }

    private async Task<GeneratePayrollViewModel> BuildGenerateViewModel(
        GeneratePayrollViewModel vm, int companyId)
    {
        var employees = await _employeeService.GetAllAsync(companyId);
        vm.Employees = employees
            .Select(e => new SelectListItem(
                $"{e.FullName} ({e.EmployeeCode})", e.Id.ToString()))
            .ToList();
        vm.Months = BuildMonthList(vm.Month);
        vm.Years = BuildYearList(vm.Year);
        return vm;
    }

    private static List<SelectListItem> BuildMonthList(int selected) =>
        Enumerable.Range(1, 12)
            .Select(m => new SelectListItem(
                new DateTime(2000, m, 1).ToString("MMMM"),
                m.ToString(), m == selected))
            .ToList();

    private static List<SelectListItem> BuildYearList(int selected) =>
        Enumerable.Range(DateTime.Today.Year - 3, 6)
            .Select(y => new SelectListItem(
                y.ToString(), y.ToString(), y == selected))
            .ToList();
}