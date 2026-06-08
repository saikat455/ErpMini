// ErpMini.Infrastructure/Services/PayrollService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Domain.Enums;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class PayrollService : IPayrollService
{
    private readonly AppDbContext _context;

    public PayrollService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<PayrollDto>> GetAllAsync(int companyId)
    {
        return await _context.Payrolls
            .Include(p => p.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Employee).ThenInclude(e => e.Designation)
            .Where(p => p.Employee.CompanyId == companyId)
            .OrderByDescending(p => p.Year)
            .ThenByDescending(p => p.Month)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<IEnumerable<PayrollDto>> GetByMonthYearAsync(
        int month, int year, int companyId)
    {
        return await _context.Payrolls
            .Include(p => p.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Employee).ThenInclude(e => e.Designation)
            .Where(p => p.Employee.CompanyId == companyId
                     && p.Month == month
                     && p.Year == year)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PayrollDto?> GetByIdAsync(int id, int companyId)
    {
        var p = await _context.Payrolls
            .Include(p => p.Employee).ThenInclude(e => e.Department)
            .Include(p => p.Employee).ThenInclude(e => e.Designation)
            .FirstOrDefaultAsync(p => p.Id == id
                                   && p.Employee.CompanyId == companyId);

        return p is null ? null : MapToDto(p);
    }

    public async Task<bool> ExistsAsync(int employeeId, int month, int year)
    {
        return await _context.Payrolls
            .AnyAsync(p => p.EmployeeId == employeeId
                        && p.Month == month
                        && p.Year == year);
    }

    public async Task<bool> GenerateAsync(GeneratePayrollDto dto)
    {
        if (await ExistsAsync(dto.EmployeeId, dto.Month, dto.Year))
            return false;

        var payroll = new Payroll
        {
            EmployeeId = dto.EmployeeId,
            Month = dto.Month,
            Year = dto.Year,
            BasicSalary = dto.BasicSalary,
            Bonus = dto.Bonus,
            Deduction = dto.Deduction,
            NetSalary = dto.BasicSalary + dto.Bonus - dto.Deduction,
            Note = dto.Note,
            Status = PayrollStatus.Generated,
            GeneratedBy = dto.GeneratedBy,
            GeneratedOn = DateTime.UtcNow,
            CompanyId = dto.CompanyId,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Payrolls.AddAsync(payroll);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> GenerateBulkAsync(
        int month, int year, string generatedBy, int companyId)
    {
        // Only active employees belonging to this company
        var employees = await _context.Employees
            .Where(e => e.CompanyId == companyId && e.IsActive)
            .ToListAsync();

        var existingPayrolls = await _context.Payrolls
            .Where(p => p.Employee.CompanyId == companyId
                     && p.Month == month
                     && p.Year == year)
            .Select(p => p.EmployeeId)
            .ToListAsync();

        var pending = employees
            .Where(e => !existingPayrolls.Contains(e.Id))
            .ToList();

        if (!pending.Any()) return false;

        var payrolls = pending.Select(e => new Payroll
        {
            EmployeeId = e.Id,
            Month = month,
            Year = year,
            BasicSalary = e.BasicSalary,
            Bonus = 0,
            Deduction = 0,
            NetSalary = e.BasicSalary,
            Status = PayrollStatus.Generated,
            GeneratedBy = generatedBy,
            GeneratedOn = DateTime.UtcNow,
            CompanyId = companyId,
            CreatedAt = DateTime.UtcNow
        });

        await _context.Payrolls.AddRangeAsync(payrolls);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> MarkAsPaidAsync(int id, int companyId)
    {
        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.Id == id
                                   && p.Employee.CompanyId == companyId);

        if (payroll is null) return false;

        payroll.Status = PayrollStatus.Paid;
        payroll.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var payroll = await _context.Payrolls
            .Include(p => p.Employee)
            .FirstOrDefaultAsync(p => p.Id == id
                                   && p.Employee.CompanyId == companyId);

        if (payroll is null) return false;

        payroll.IsDeleted = true;
        payroll.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<PayrollSummaryDto>> GetSummaryAsync(int companyId)
    {
        return await _context.Payrolls
            .Where(p => p.Employee.CompanyId == companyId)
            .GroupBy(p => new { p.Month, p.Year })
            .Select(g => new PayrollSummaryDto
            {
                Month = g.Key.Month,
                Year = g.Key.Year,
                TotalEmployees = g.Count(),
                TotalBasic = g.Sum(p => p.BasicSalary),
                TotalBonus = g.Sum(p => p.Bonus),
                TotalDeduction = g.Sum(p => p.Deduction),
                TotalNet = g.Sum(p => p.NetSalary)
            })
            .OrderByDescending(s => s.Year)
            .ThenByDescending(s => s.Month)
            .ToListAsync();
    }

    private static PayrollDto MapToDto(Payroll p) => new()
    {
        Id = p.Id,
        EmployeeId = p.EmployeeId,
        EmployeeName = $"{p.Employee.FirstName} {p.Employee.LastName}",
        EmployeeCode = p.Employee.EmployeeCode,
        DepartmentName = p.Employee.Department.Name,
        DesignationTitle = p.Employee.Designation.Title,
        Month = p.Month,
        Year = p.Year,
        BasicSalary = p.BasicSalary,
        Bonus = p.Bonus,
        Deduction = p.Deduction,
        NetSalary = p.NetSalary,
        Note = p.Note,
        Status = p.Status,
        GeneratedOn = p.GeneratedOn,
        GeneratedBy = p.GeneratedBy
    };
}