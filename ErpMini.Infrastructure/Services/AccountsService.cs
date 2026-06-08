// ErpMini.Infrastructure/Services/AccountsService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Domain.Enums;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class AccountsService : IAccountsService
{
    private readonly AppDbContext _context;
    public AccountsService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<TransactionDto>> GetAllAsync(int companyId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CompanyId == companyId)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<IEnumerable<TransactionDto>> GetByTypeAsync(
        TransactionType type, int companyId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CompanyId == companyId && t.Type == type)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(
        DateTime from, DateTime to, int companyId)
    {
        return await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CompanyId == companyId
                     && t.TransactionDate >= from
                     && t.TransactionDate <= to)
            .OrderByDescending(t => t.TransactionDate)
            .Select(t => MapToDto(t))
            .ToListAsync();
    }

    public async Task<bool> CreateAsync(CreateTransactionDto dto)
    {
        var txn = new Transaction
        {
            ReferenceNo     = await GenerateReferenceNoAsync(dto.CompanyId),
            CategoryId      = dto.CategoryId,
            CompanyId       = dto.CompanyId,
            Type            = dto.Type,
            Amount          = dto.Amount,
            TransactionDate = DateTime.SpecifyKind(dto.TransactionDate, DateTimeKind.Utc),
            Description     = dto.Description,
            Note            = dto.Note,
            CreatedByUser   = dto.CreatedByUser,
            CreatedAt       = DateTime.UtcNow
        };

        await _context.Transactions.AddAsync(txn);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var txn = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id && t.CompanyId == companyId);

        if (txn is null) return false;
        txn.IsDeleted = true;
        txn.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<string> GenerateReferenceNoAsync(int companyId)
    {
        var last = await _context.Transactions
            .IgnoreQueryFilters()
            .Where(t => t.CompanyId == companyId)
            .OrderByDescending(t => t.Id)
            .Select(t => t.ReferenceNo)
            .FirstOrDefaultAsync();

        if (last is null) return "TXN-0001";
        var num = int.Parse(last.Split('-')[1]);
        return $"TXN-{(num + 1):D4}";
    }

    public async Task<IEnumerable<LedgerDto>> GetLedgerAsync(
        DateTime? from, DateTime? to, int companyId)
    {
        var query = _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CompanyId == companyId)
            .AsQueryable();

        if (from.HasValue)
            query = query.Where(t => t.TransactionDate >= from.Value);
        if (to.HasValue)
            query = query.Where(t => t.TransactionDate <= to.Value);

        var transactions = await query
            .OrderBy(t => t.TransactionDate)
            .ToListAsync();

        // Build running balance
        decimal running = 0;
        var ledger = new List<LedgerDto>();

        foreach (var t in transactions)
        {
            if (t.Type == TransactionType.Income)
                running += t.Amount;
            else
                running -= t.Amount;

            ledger.Add(new LedgerDto
            {
                ReferenceNo     = t.ReferenceNo,
                TransactionDate = t.TransactionDate,
                Description     = t.Description,
                CategoryName    = t.Category.Name,
                Income          = t.Type == TransactionType.Income  ? t.Amount : 0,
                Expense         = t.Type == TransactionType.Expense ? t.Amount : 0,
                Balance         = running
            });
        }

        return ledger;
    }

    public async Task<IEnumerable<FinancialSummaryDto>> GetMonthlySummaryAsync(int companyId)
    {
        return await _context.Transactions
            .Where(t => t.CompanyId == companyId)
            .GroupBy(t => new { t.TransactionDate.Year, t.TransactionDate.Month })
            .Select(g => new FinancialSummaryDto
            {
                Year         = g.Key.Year,
                Month        = g.Key.Month,
                TotalIncome  = g.Where(t => t.Type == TransactionType.Income)
                                .Sum(t => t.Amount),
                TotalExpense = g.Where(t => t.Type == TransactionType.Expense)
                                .Sum(t => t.Amount)
            })
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ToListAsync();
    }

    public async Task<(decimal income, decimal expense, decimal balance)> GetTotalsAsync(
        int companyId)
    {
        var income = await _context.Transactions
            .Where(t => t.CompanyId == companyId && t.Type == TransactionType.Income)
            .SumAsync(t => t.Amount);

        var expense = await _context.Transactions
            .Where(t => t.CompanyId == companyId && t.Type == TransactionType.Expense)
            .SumAsync(t => t.Amount);

        return (income, expense, income - expense);
    }

    // ── Categories are global (not company-scoped) ────────────────────────────
    public async Task<IEnumerable<AccountCategoryDto>> GetCategoriesAsync()
    {
        return await _context.AccountCategories
            .Select(c => new AccountCategoryDto
            {
                Id               = c.Id,
                Name             = c.Name,
                Type             = c.Type,
                Description      = c.Description,
                IsActive         = c.IsActive,
                TransactionCount = c.Transactions.Count(t => !t.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<bool> CreateCategoryAsync(
        string name, string type, string? description, int companyId)
    {
        await _context.AccountCategories.AddAsync(new AccountCategory
        {
            Name        = name,
            Type        = type,
            Description = description,
            CompanyId   = companyId,
            CreatedAt   = DateTime.UtcNow
        });
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        var c = await _context.AccountCategories.FindAsync(id);
        if (c is null) return false;
        c.IsDeleted = true;
        c.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    private static TransactionDto MapToDto(Transaction t) => new()
    {
        Id              = t.Id,
        ReferenceNo     = t.ReferenceNo,
        CategoryId      = t.CategoryId,
        CategoryName    = t.Category.Name,
        Type            = t.Type,
        Amount          = t.Amount,
        TransactionDate = t.TransactionDate,
        Description     = t.Description,
        Note            = t.Note,
        CreatedByUser   = t.CreatedByUser,
        CreatedAt       = t.CreatedAt
    };
}