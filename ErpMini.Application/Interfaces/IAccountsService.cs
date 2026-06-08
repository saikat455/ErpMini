// ErpMini.Application/Interfaces/IAccountsService.cs
using ErpMini.Application.DTOs;
using ErpMini.Domain.Enums;

namespace ErpMini.Application.Interfaces;

public interface IAccountsService
{
    // Transactions
    Task<IEnumerable<TransactionDto>> GetAllAsync(int companyId);
    Task<IEnumerable<TransactionDto>> GetByTypeAsync(TransactionType type, int companyId);
    Task<IEnumerable<TransactionDto>> GetByDateRangeAsync(DateTime from, DateTime to, int companyId);
    Task<bool> CreateAsync(CreateTransactionDto dto);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<string> GenerateReferenceNoAsync(int companyId);

    // Ledger
    Task<IEnumerable<LedgerDto>> GetLedgerAsync(DateTime? from, DateTime? to, int companyId);

    // Summary
    Task<IEnumerable<FinancialSummaryDto>> GetMonthlySummaryAsync(int companyId);
    Task<(decimal income, decimal expense, decimal balance)> GetTotalsAsync(int companyId);

    // Categories (global — not company-scoped)
    Task<IEnumerable<AccountCategoryDto>> GetCategoriesAsync();
    Task<bool> CreateCategoryAsync(string name, string type, string? description, int companyId);
    Task<bool> DeleteCategoryAsync(int id);
}