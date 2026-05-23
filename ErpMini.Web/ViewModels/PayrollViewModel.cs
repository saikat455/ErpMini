// ErpMini.Web/ViewModels/PayrollViewModel.cs
using ErpMini.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ErpMini.Web.ViewModels;

public class GeneratePayrollViewModel
{
    [Required]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    [Required]
    [Range(1, 12)]
    public int Month { get; set; } = DateTime.Today.Month;

    [Required]
    [Range(2020, 2100)]
    public int Year { get; set; } = DateTime.Today.Year;

    [Required]
    [Range(0, double.MaxValue)]
    [Display(Name = "Basic Salary")]
    public decimal BasicSalary { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Bonus { get; set; }

    [Range(0, double.MaxValue)]
    public decimal Deduction { get; set; }

    public string? Note { get; set; }

    // Computed
    public decimal NetSalary => BasicSalary + Bonus - Deduction;

    // Dropdowns
    public List<SelectListItem> Employees { get; set; } = new();
    public List<SelectListItem> Months { get; set; } = new();
    public List<SelectListItem> Years { get; set; } = new();
}

public class PayrollFilterViewModel
{
    public int Month { get; set; } = DateTime.Today.Month;
    public int Year { get; set; } = DateTime.Today.Year;
    public List<SelectListItem> Months { get; set; } = new();
    public List<SelectListItem> Years { get; set; } = new();
    public IEnumerable<PayrollDto> Payrolls { get; set; } = new List<PayrollDto>();
}