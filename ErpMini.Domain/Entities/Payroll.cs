// ErpMini.Domain/Entities/Payroll.cs
namespace ErpMini.Domain.Entities;
using ErpMini.Domain.Enums;
public class Payroll : BaseEntity
{
    public int EmployeeId { get; set; }
    public int Month { get; set; }           // 1-12
    public int Year { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal Bonus { get; set; }
    public decimal Deduction { get; set; }
    public decimal NetSalary { get; set; }   // BasicSalary + Bonus - Deduction
    public string? Note { get; set; }
    public PayrollStatus Status { get; set; } = PayrollStatus.Generated;
    public DateTime GeneratedOn { get; set; } = DateTime.UtcNow;
    public string? GeneratedBy { get; set; }

    // Navigation
    public Employee Employee { get; set; } = null!;
}