// ErpMini.Application/DTOs/DashboardDto.cs
namespace ErpMini.Application.DTOs;

public class DashboardDto
{
    // HR
    public int TotalEmployees { get; set; }
    public int ActiveEmployees { get; set; }
    public int TotalDepartments { get; set; }

    // Leave
    public int PendingLeaves { get; set; }
    public int ApprovedLeavesThisMonth { get; set; }

    // Payroll
    public decimal TotalPayrollThisMonth { get; set; }
    public int PayrollEmployeesThisMonth { get; set; }

    // Accounts
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetBalance => TotalIncome - TotalExpense;

    // Procurement
    public int PendingOrders { get; set; }
    public int TotalVendors { get; set; }

    // Charts
    public List<MonthlyFinancialData> MonthlyFinancials { get; set; } = new();
    public List<DepartmentHeadcount> DepartmentHeadcounts { get; set; } = new();
    public List<RecentTransaction> RecentTransactions { get; set; } = new();
}

public class MonthlyFinancialData
{
    public string Month { get; set; } = string.Empty;
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
}

public class DepartmentHeadcount
{
    public string Department { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class RecentTransaction
{
    public string ReferenceNo { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
}