// ErpMini.Web/ViewModels/AccountsViewModel.cs
using ErpMini.Application.DTOs;
using ErpMini.Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ErpMini.Web.ViewModels;

public class CreateTransactionViewModel
{
    [Required]
    [Display(Name = "Transaction Type")]
    public TransactionType Type { get; set; }

    [Required]
    [Display(Name = "Category")]
    public int CategoryId { get; set; }

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required]
    [Display(Name = "Transaction Date")]
    public DateTime TransactionDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(300, MinimumLength = 3)]
    public string Description { get; set; } = string.Empty;

    public string? Note { get; set; }

    public List<SelectListItem> Categories { get; set; } = new();
}

public class LedgerFilterViewModel
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public IEnumerable<LedgerDto> Entries { get; set; } = new List<LedgerDto>();
}

public class TransactionFilterViewModel
{
    public string? TypeFilter { get; set; }   // "Income", "Expense", or null = All
    public IEnumerable<TransactionDto> Transactions { get; set; } = new List<TransactionDto>();
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal Balance => TotalIncome - TotalExpense;
}