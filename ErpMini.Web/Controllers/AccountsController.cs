// ErpMini.Web/Controllers/AccountsController.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Enums;
using ErpMini.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ErpMini.Web.Controllers;

[Authorize]
public class AccountsController : Controller
{
    private readonly IAccountsService _service;
    public AccountsController(IAccountsService service) => _service = service;

    // All transactions with filter
    public async Task<IActionResult> Index(string? typeFilter)
    {
        var (income, expense, _) = await _service.GetTotalsAsync();

        IEnumerable<TransactionDto> transactions = typeFilter switch
        {
            "Income"  => await _service.GetByTypeAsync(TransactionType.Income),
            "Expense" => await _service.GetByTypeAsync(TransactionType.Expense),
            _         => await _service.GetAllAsync()
        };

        var vm = new TransactionFilterViewModel
        {
            TypeFilter = typeFilter,
            Transactions = transactions,
            TotalIncome = income,
            TotalExpense = expense
        };

        return View(vm);
    }

    // Add income
    public async Task<IActionResult> AddIncome()
    {
        var vm = new CreateTransactionViewModel { Type = TransactionType.Income };
        return View(await BuildViewModel(vm, "Income"));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> AddIncome(CreateTransactionViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, "Income"));

        await _service.CreateAsync(new CreateTransactionDto
        {
            CategoryId = vm.CategoryId,
            Type = TransactionType.Income,
            Amount = vm.Amount,
            TransactionDate = vm.TransactionDate,
            Description = vm.Description,
            Note = vm.Note,
            CreatedByUser = User.Identity?.Name ?? "Admin"
        });

        TempData["Success"] = "Income entry added.";
        return RedirectToAction(nameof(Index), new { typeFilter = "Income" });
    }

    // Add expense
    public async Task<IActionResult> AddExpense()
    {
        var vm = new CreateTransactionViewModel { Type = TransactionType.Expense };
        return View(await BuildViewModel(vm, "Expense"));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> AddExpense(CreateTransactionViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(await BuildViewModel(vm, "Expense"));

        await _service.CreateAsync(new CreateTransactionDto
        {
            CategoryId = vm.CategoryId,
            Type = TransactionType.Expense,
            Amount = vm.Amount,
            TransactionDate = vm.TransactionDate,
            Description = vm.Description,
            Note = vm.Note,
            CreatedByUser = User.Identity?.Name ?? "Admin"
        });

        TempData["Success"] = "Expense entry added.";
        return RedirectToAction(nameof(Index), new { typeFilter = "Expense" });
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        TempData["Success"] = "Transaction deleted.";
        return RedirectToAction(nameof(Index));
    }

    // Ledger
    public async Task<IActionResult> Ledger(DateTime? from, DateTime? to)
    {
        var fromDate = from ?? new DateTime(DateTime.Today.Year, 1, 1);
        var toDate   = to   ?? DateTime.Today;

        var entries = await _service.GetLedgerAsync(
            DateTime.SpecifyKind(fromDate, DateTimeKind.Utc),
            DateTime.SpecifyKind(toDate,   DateTimeKind.Utc));

        var vm = new LedgerFilterViewModel
        {
            From    = fromDate,
            To      = toDate,
            Entries = entries
        };

        return View(vm);
    }

    // Monthly summary
    public async Task<IActionResult> Summary()
    {
        var summary = await _service.GetMonthlySummaryAsync();
        return View(summary);
    }

    // Category management
    public async Task<IActionResult> Categories()
        => View(await _service.GetCategoriesAsync());

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCategory(
        string name, string type, string? description)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            TempData["Error"] = "Category name is required.";
            return RedirectToAction(nameof(Categories));
        }
        await _service.CreateCategoryAsync(name, type, description);
        TempData["Success"] = "Category added.";
        return RedirectToAction(nameof(Categories));
    }

    [HttpPost][ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        await _service.DeleteCategoryAsync(id);
        TempData["Success"] = "Category deleted.";
        return RedirectToAction(nameof(Categories));
    }

    private async Task<CreateTransactionViewModel> BuildViewModel(
        CreateTransactionViewModel vm, string typeFilter)
    {
        var categories = await _service.GetCategoriesAsync();
        vm.Categories = categories
            .Where(c => c.Type == typeFilter && c.IsActive)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToList();
        return vm;
    }
}