// ErpMini.Infrastructure/Services/LeaveService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Domain.Enums;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class LeaveService : ILeaveService
{
    private readonly AppDbContext _context;

    public LeaveService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<LeaveApplicationDto>> GetAllAsync()
    {
        return await _context.LeaveApplications
            .Include(l => l.Employee).ThenInclude(e => e.Department)
            .Include(l => l.LeaveType)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => MapToDto(l))
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveApplicationDto>> GetPendingAsync()
    {
        return await _context.LeaveApplications
            .Include(l => l.Employee).ThenInclude(e => e.Department)
            .Include(l => l.LeaveType)
            .Where(l => l.Status == LeaveStatus.Pending)
            .OrderBy(l => l.CreatedAt)
            .Select(l => MapToDto(l))
            .ToListAsync();
    }

    public async Task<IEnumerable<LeaveApplicationDto>> GetByEmployeeAsync(int employeeId)
    {
        return await _context.LeaveApplications
            .Include(l => l.Employee)
            .Include(l => l.LeaveType)
            .Where(l => l.EmployeeId == employeeId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => MapToDto(l))
            .ToListAsync();
    }

    public async Task<bool> ApplyAsync(CreateLeaveDto dto)
    {
        // Calculate working days (simple version — excludes weekends)
        var totalDays = 0;
        for (var date = dto.FromDate; date <= dto.ToDate; date = date.AddDays(1))
        {
            if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                totalDays++;
        }

        var application = new LeaveApplication
        {
            EmployeeId = dto.EmployeeId,
            LeaveTypeId = dto.LeaveTypeId,
            FromDate = dto.FromDate,
            ToDate = dto.ToDate,
            TotalDays = totalDays,
            Reason = dto.Reason,
            Status = LeaveStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        await _context.LeaveApplications.AddAsync(application);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ApproveAsync(int id, string actionBy)
    {
        var leave = await _context.LeaveApplications.FindAsync(id);
        if (leave is null || leave.Status != LeaveStatus.Pending) return false;

        leave.Status = LeaveStatus.Approved;
        leave.ActionBy = actionBy;
        leave.ActionDate = DateTime.UtcNow;
        leave.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RejectAsync(int id, string reason, string actionBy)
    {
        var leave = await _context.LeaveApplications.FindAsync(id);
        if (leave is null || leave.Status != LeaveStatus.Pending) return false;

        leave.Status = LeaveStatus.Rejected;
        leave.RejectionReason = reason;
        leave.ActionBy = actionBy;
        leave.ActionDate = DateTime.UtcNow;
        leave.UpdatedAt = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<IEnumerable<LeaveBalanceDto>> GetBalanceAsync(int employeeId)
    {
        var leaveTypes = await _context.LeaveTypes.ToListAsync();
        var currentYear = DateTime.UtcNow.Year;

        var usedLeaves = await _context.LeaveApplications
            .Where(l => l.EmployeeId == employeeId
                     && l.Status == LeaveStatus.Approved
                     && l.FromDate.Year == currentYear)
            .GroupBy(l => l.LeaveTypeId)
            .Select(g => new { LeaveTypeId = g.Key, Used = g.Sum(l => l.TotalDays) })
            .ToListAsync();

        return leaveTypes.Select(lt => new LeaveBalanceDto
        {
            LeaveTypeName = lt.Name,
            Allocated = lt.TotalDays,
            Used = usedLeaves.FirstOrDefault(u => u.LeaveTypeId == lt.Id)?.Used ?? 0
        });
    }

    public async Task<IEnumerable<LeaveTypeDto>> GetLeaveTypesAsync()
    {
        return await _context.LeaveTypes
            .Select(lt => new LeaveTypeDto
            {
                Id = lt.Id, Name = lt.Name,
                Description = lt.Description,
                TotalDays = lt.TotalDays, IsActive = lt.IsActive
            }).ToListAsync();
    }

    public async Task<bool> CreateLeaveTypeAsync(string name, int days, string? description)
    {
        await _context.LeaveTypes.AddAsync(new LeaveType
        {
            Name = name, TotalDays = days,
            Description = description, CreatedAt = DateTime.UtcNow
        });
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteLeaveTypeAsync(int id)
    {
        var lt = await _context.LeaveTypes.FindAsync(id);
        if (lt is null) return false;
        lt.IsDeleted = true; lt.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    // Private mapper keeps Select() expressions clean
    private static LeaveApplicationDto MapToDto(LeaveApplication l) => new()
    {
        Id = l.Id,
        EmployeeId = l.EmployeeId,
        EmployeeName = $"{l.Employee.FirstName} {l.Employee.LastName}",
        EmployeeCode = l.Employee.EmployeeCode,
        DepartmentName = l.Employee.Department.Name,
        LeaveTypeId = l.LeaveTypeId,
        LeaveTypeName = l.LeaveType.Name,
        FromDate = l.FromDate,
        ToDate = l.ToDate,
        TotalDays = l.TotalDays,
        Reason = l.Reason,
        Status = l.Status,
        RejectionReason = l.RejectionReason,
        ActionDate = l.ActionDate,
        ActionBy = l.ActionBy,
        CreatedAt = l.CreatedAt
    };
}