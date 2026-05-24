// ErpMini.Application/Interfaces/IAccountsService.cs
using ErpMini.Application.DTOs;
using ErpMini.Domain.Enums;

namespace ErpMini.Application.Interfaces;

public interface IAccountsService
{
    // Transactions
    Task<IEnumerable<TransactionDto>> GetAllAsync();
    Task<IEnumerable<TransactionDto>> GetByTypeAsync(TransactionType type);
    Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(DateTime from, DateTime to);
    Task<bool> CreateAsync(CreateTransactionDto dto);
    Task<bool> DeleteAsync(int id);
    Task<string> GenerateReferenceNoAsync();

    // Ledger
    Task<IEnumerable<LedgerDto>> GetLedgerAsync(DateTime? from, DateTime? to);

    // Summary
    Task<IEnumerable<FinancialSummaryDto>> GetMonthlySummaryAsync();
    Task<(decimal income, decimal expense, decimal balance)> GetTotalsAsync();

    // Categories
    Task<IEnumerable<AccountCategoryDto>> GetCategoriesAsync();
    Task<bool> CreateCategoryAsync(string name, string type, string? description);
    Task<bool> DeleteCategoryAsync(int id);
}