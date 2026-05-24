// ErpMini.Domain/Entities/AccountCategory.cs
namespace ErpMini.Domain.Entities;

public class AccountCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;      // e.g. Sales, Rent, Utilities
    public string Type { get; set; } = string.Empty;      // "Income" or "Expense"
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}