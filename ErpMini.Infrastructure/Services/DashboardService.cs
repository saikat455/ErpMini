// ErpMini.Infrastructure/Services/DashboardService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Enums;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    public DashboardService(AppDbContext context) => _context = context;

    public async Task<DashboardDto> GetDashboardDataAsync(int companyId)
    {
        var now          = DateTime.UtcNow;
        var currentMonth = now.Month;
        var currentYear  = now.Year;

        // ── HR ────────────────────────────────────────────────────
        var totalEmployees  = await _context.Employees
            .CountAsync(e => e.CompanyId == companyId);

        var activeEmployees = await _context.Employees
            .CountAsync(e => e.CompanyId == companyId && e.IsActive);

        var totalDepts = await _context.Departments
            .CountAsync(d => d.CompanyId == companyId);

        // ── Leave ─────────────────────────────────────────────────
        var pendingLeaves = await _context.LeaveApplications
            .CountAsync(l => l.Employee.CompanyId == companyId
                          && l.Status == LeaveStatus.Pending);

        var approvedThisMonth = await _context.LeaveApplications
            .CountAsync(l => l.Employee.CompanyId == companyId
                          && l.Status == LeaveStatus.Approved
                          && l.FromDate.Month == currentMonth
                          && l.FromDate.Year  == currentYear);

        // ── Payroll ───────────────────────────────────────────────
        var payrollThisMonth = await _context.Payrolls
            .Where(p => p.Employee.CompanyId == companyId
                     && p.Month == currentMonth
                     && p.Year  == currentYear)
            .ToListAsync();

        // ── Accounts ──────────────────────────────────────────────
        var totalIncome = await _context.Transactions
            .Where(t => t.CompanyId == companyId
                     && t.Type == TransactionType.Income)
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

        var totalExpense = await _context.Transactions
            .Where(t => t.CompanyId == companyId
                     && t.Type == TransactionType.Expense)
            .SumAsync(t => (decimal?)t.Amount) ?? 0;

        // ── Procurement ───────────────────────────────────────────
        var pendingOrders = await _context.PurchaseOrders
            .CountAsync(p => p.CompanyId == companyId
                          && (p.Status == PurchaseOrderStatus.Submitted
                           || p.Status == PurchaseOrderStatus.Draft));

        var totalVendors = await _context.Vendors
            .CountAsync(v => v.CompanyId == companyId);

        // ── Monthly financials (last 6 months) ────────────────────
        var monthlyData = new List<MonthlyFinancialData>();
        for (int i = 5; i >= 0; i--)
        {
            var date = now.AddMonths(-i);
            var m    = date.Month;
            var y    = date.Year;

            var inc = await _context.Transactions
                .Where(t => t.CompanyId == companyId
                         && t.Type == TransactionType.Income
                         && t.TransactionDate.Month == m
                         && t.TransactionDate.Year  == y)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            var exp = await _context.Transactions
                .Where(t => t.CompanyId == companyId
                         && t.Type == TransactionType.Expense
                         && t.TransactionDate.Month == m
                         && t.TransactionDate.Year  == y)
                .SumAsync(t => (decimal?)t.Amount) ?? 0;

            monthlyData.Add(new MonthlyFinancialData
            {
                Month   = date.ToString("MMM"),
                Income  = inc,
                Expense = exp
            });
        }

        // ── Department headcount ──────────────────────────────────
        var deptCounts = await _context.Employees
            .Where(e => e.CompanyId == companyId && e.IsActive)
            .GroupBy(e => e.Department.Name)
            .Select(g => new DepartmentHeadcount
            {
                Department = g.Key,
                Count      = g.Count()
            })
            .ToListAsync();

        // ── Recent transactions (last 5) ──────────────────────────
        var recentTxns = await _context.Transactions
            .Include(t => t.Category)
            .Where(t => t.CompanyId == companyId)
            .OrderByDescending(t => t.TransactionDate)
            .Take(5)
            .Select(t => new RecentTransaction
            {
                ReferenceNo = t.ReferenceNo,
                Description = t.Description,
                Type        = t.Type.ToString(),
                Amount      = t.Amount,
                Date        = t.TransactionDate
            })
            .ToListAsync();

        return new DashboardDto
        {
            TotalEmployees            = totalEmployees,
            ActiveEmployees           = activeEmployees,
            TotalDepartments          = totalDepts,
            PendingLeaves             = pendingLeaves,
            ApprovedLeavesThisMonth   = approvedThisMonth,
            TotalPayrollThisMonth     = payrollThisMonth.Sum(p => p.NetSalary),
            PayrollEmployeesThisMonth = payrollThisMonth.Count,
            TotalIncome               = totalIncome,
            TotalExpense              = totalExpense,
            PendingOrders             = pendingOrders,
            TotalVendors              = totalVendors,
            MonthlyFinancials         = monthlyData,
            DepartmentHeadcounts      = deptCounts,
            RecentTransactions        = recentTxns
        };
    }
}