// ErpMini.Application/DTOs/AccountsDto.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Application.DTOs;

public class TransactionDto
{
    public int Id { get; set; }
    public string ReferenceNo { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string TypeLabel => Type.ToString();
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateTransactionDto
{
    public int CategoryId { get; set; }
    public TransactionType Type { get; set; }
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    public int CompanyId { get; set; }
}

public class AccountCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public int TransactionCount { get; set; }
}

public class LedgerDto
{
    public string ReferenceNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }   // running balance
}

public class FinancialSummaryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance => TotalIncome - TotalExpense;
}