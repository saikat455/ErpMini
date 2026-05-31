// ErpMini.Domain/Entities/Transaction.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Domain.Entities;

public class Transaction : BaseEntity
{
    public string ReferenceNo { get; set; } = string.Empty;   // e.g. TXN-001
    public int CategoryId { get; set; }
    public TransactionType Type { get; set; }                  // Income or Expense
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? Note { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;
    // public int CompanyId { get; set; }
    // Navigation
    public AccountCategory Category { get; set; } = null!;
}