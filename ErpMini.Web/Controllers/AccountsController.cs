// ErpMini.Web/Controllers/AccountsController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Enums;
using ErpMini.Web.Helpers;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AccountsController : BaseController
{
    private readonly IAccountsService _service;

    public AccountsController(
        IAccountsService service,
        UserContext userContext) : base(userContext)
    {
        _service = service;
    }

    public async Task<IActionResult> Index(string? typeFilter)
    {
        var companyId = await GetCompanyIdAsync();
        var (income, expense, _) = await _service.GetTotalsAsync(companyId);

        IEnumerable<TransactionDto> transactions = typeFilter switch
        {
            "Income" => await _service.GetByTypeAsync(TransactionType.Income, companyId),
            "Expense" => await _service.GetByTypeAsync(TransactionType.Expense, companyId),
            _ => await _service.GetAllAsync(companyId)
        };

        return View(new TransactionFilterViewModel
        {
            TypeFilter = typeFilter,
            Transactions = transactions,
            TotalIncome = income,
            TotalExpense = expense
        });
    }

    public async Task<IActionResult> AddIncome()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await BuildViewModel(
            new CreateTransactionViewModel
            { Type = TransactionType.Income }, "Income", companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIncome(CreateTransactionViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, "Income", companyId));

        await _service.CreateAsync(new CreateTransactionDto
        {
            CategoryId = vm.CategoryId,
            Type = TransactionType.Income,
            Amount = vm.Amount,
            TransactionDate = vm.TransactionDate,
            Description = vm.Description,
            Note = vm.Note,
            CreatedByUser = User.Identity?.Name ?? "Admin",
            CompanyId = companyId
        });

        TempData["Success"] = "Income added.";
        return RedirectToAction(nameof(Index), new { typeFilter = "Income" });
    }

    public async Task<IActionResult> AddExpense()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await BuildViewModel(
            new CreateTransactionViewModel
            { Type = TransactionType.Expense }, "Expense", companyId));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(CreateTransactionViewModel vm)
    {
        var companyId = await GetCompanyIdAsync();
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, "Expense", companyId));

        await _service.CreateAsync(new CreateTransactionDto
        {
            CategoryId = vm.CategoryId,
            Type = TransactionType.Expense,
            Amount = vm.Amount,
            TransactionDate = vm.TransactionDate,
            Description = vm.Description,
            Note = vm.Note,
            CreatedByUser = User.Identity?.Name ?? "Admin",
            CompanyId = companyId
        });

        TempData["Success"] = "Expense added.";
        return RedirectToAction(nameof(Index), new { typeFilter = "Expense" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var companyId = await GetCompanyIdAsync();
        await _service.DeleteAsync(id, companyId);
        TempData["Success"] = "Transaction deleted.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Ledger(DateTime? from, DateTime? to)
    {
        var companyId = await GetCompanyIdAsync();
        var fromDate = from ?? new DateTime(DateTime.Today.Year, 1, 1);
        var toDate = to ?? DateTime.Today;

        var entries = await _service.GetLedgerAsync(
            DateTime.SpecifyKind(fromDate, DateTimeKind.Utc),
            DateTime.SpecifyKind(toDate, DateTimeKind.Utc),
            companyId);

        return View(new LedgerFilterViewModel
        {
            From = fromDate,
            To = toDate,
            Entries = entries
        });
    }

    public async Task<IActionResult> Summary()
    {
        var companyId = await GetCompanyIdAsync();
        return View(await _service.GetMonthlySummaryAsync(companyId));
    }

    public async Task<IActionResult> Categories()
        => View(await _service.GetCategoriesAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(
        string name, string type, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Name is required.";
            return RedirectToAction(nameof(Categories));
        }
        var companyId = await GetCompanyIdAsync();
        await _service.CreateCategoryAsync(name, type, description, companyId);
        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _service.DeleteCategoryAsync(id);
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Categories));
    }

    private async Task<CreateTransactionViewModel> BuildViewModel(
        CreateTransactionViewModel vm, string typeFilter, int companyId)
    {
        var categories = await _service.GetCategoriesAsync();
        vm.Categories = categories
            .Where(c => c.Type == typeFilter && c.IsActive)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
        return vm;
    }
}