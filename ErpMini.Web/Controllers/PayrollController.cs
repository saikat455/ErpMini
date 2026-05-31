// ErpMini.Web/Controllers/PayrollController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin,Finance")]
public class PayrollController : Controller
{
    private readonly IPayrollService _payrollService;
    private readonly IEmployeeService _employeeService;

    public PayrollController(IPayrollService payroll, IEmployeeService employee)
    {
        _payrollService = payroll;
        _employeeService = employee;
    }

    // Salary sheet filtered by month/year
    public async Task<IActionResult> Index(int? month, int? year)
    {
        var selectedMonth = month ?? DateTime.Today.Month;
        var selectedYear = year ?? DateTime.Today.Year;

        var payrolls = await _payrollService.GetByMonthYearAsync(selectedMonth, selectedYear);

        var vm = new PayrollFilterViewModel
        {
            Month = selectedMonth,
            Year = selectedYear,
            Payrolls = payrolls,
            Months = BuildMonthList(selectedMonth),
            Years = BuildYearList(selectedYear)
        };

        return View(vm);
    }

    // All payrolls history
    public async Task<IActionResult> History()
    {
        var payrolls = await _payrollService.GetAllAsync();
        return View(payrolls);
    }

    // Monthly summary
    public async Task<IActionResult> Summary()
    {
        var summary = await _payrollService.GetSummaryAsync();
        return View(summary);
    }

    // Payslip for single employee
    public async Task<IActionResult> Payslip(int id)
    {
        var payroll = await _payrollService.GetByIdAsync(id);
        if (payroll is null) return NotFound();
        return View(payroll);
    }

    // Generate single payroll form
    public async Task<IActionResult> Generate()
    {
        return View(await BuildGenerateViewModel(new GeneratePayrollViewModel()));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GeneratePayrollViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(await BuildGenerateViewModel(vm));

        // Check duplicate
        if (await _payrollService.ExistsAsync(vm.EmployeeId, vm.Month, vm.Year))
        {
            ModelState.AddModelError("", "Payroll already exists for this employee in the selected month.");
            return View(await BuildGenerateViewModel(vm));
        }

        var dto = new GeneratePayrollDto
        {
            EmployeeId = vm.EmployeeId,
            Month = vm.Month,
            Year = vm.Year,
            BasicSalary = vm.BasicSalary,
            Bonus = vm.Bonus,
            Deduction = vm.Deduction,
            Note = vm.Note,
            GeneratedBy = User.Identity?.Name ?? "Admin"
        };

        var success = await _payrollService.GenerateAsync(dto);
        if (success)
        {
            TempData["Success"] = "Payroll generated successfully.";
            return RedirectToAction(nameof(Index));
        }

        ModelState.AddModelError("", "Failed to generate payroll.");
        return View(await BuildGenerateViewModel(vm));
    }

    // Bulk generate for all active employees
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> BulkGenerate(int month, int year)
    {
        var generatedBy = User.Identity?.Name ?? "Admin";
        var success = await _payrollService.GenerateBulkAsync(month, year, generatedBy);

        TempData[success ? "Success" : "Error"] = success
            ? $"Payroll generated for all active employees — {new DateTime(year, month, 1):MMMM yyyy}."
            : "All employees already have payroll for this month.";

        return RedirectToAction(nameof(Index), new { month, year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsPaid(int id, int month, int year)
    {
        await _payrollService.MarkAsPaidAsync(id);
        TempData["Success"] = "Marked as paid.";
        return RedirectToAction(nameof(Index), new { month, year });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _payrollService.DeleteAsync(id);
        TempData["Success"] = "Payroll deleted.";
        return RedirectToAction(nameof(Index));
    }

    // AJAX: load employee salary when employee is selected
    [HttpGet]
    public async Task<IActionResult> GetEmployeeSalary(int employeeId)
    {
        var emp = await _employeeService.GetByIdAsync(employeeId);
        if (emp is null) return Json(new { salary = 0 });
        return Json(new { salary = emp.BasicSalary });
    }

    // Helpers
    private async Task<GeneratePayrollViewModel> BuildGenerateViewModel(GeneratePayrollViewModel vm)
    {
        var employees = await _employeeService.GetAllAsync();
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
                m.ToString(),
                m == selected))
            .ToList();

    private static List<SelectListItem> BuildYearList(int selected) =>
        Enumerable.Range(DateTime.Today.Year - 3, 6)
            .Select(y => new SelectListItem(
                y.ToString(), y.ToString(), y == selected))
            .ToList();
}