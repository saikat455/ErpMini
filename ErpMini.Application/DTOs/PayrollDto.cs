// ErpMini.Application/DTOs/PayrollDto.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Application.DTOs;

public class PayrollDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public string DesignationTitle { get; set; } = string.Empty;
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public decimal BasicSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public decimal NetSalary { get; set; }
    public string? Note { get; set; }
    public PayrollStatus Status { get; set; }
    public string StatusLabel => Status.ToString();
    public DateTime GeneratedOn { get; set; }
    public string? GeneratedBy { get; set; }
}

public class GeneratePayrollDto
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public string? Note { get; set; }
    public string? GeneratedBy { get; set; }
    public int CompanyId { get; set; }
}

public class PayrollSummaryDto
{
    public int Month { get; set; }
    public int Year { get; set; }
    public string MonthName => new DateTime(Year, Month, 1).ToString("MMMM yyyy");
    public int TotalEmployees { get; set; }
    public decimal TotalBasic { get; set; }
    public decimal TotalBonus { get; set; }
    public decimal TotalDeduction { get; set; }
    public decimal TotalNet { get; set; }
}