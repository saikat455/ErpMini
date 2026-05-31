// ErpMini.Application/Interfaces/ILeaveService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface ILeaveService
{
    // Employee actions
    Task<IEnumerable<LeaveApplicationDto>> GetByEmployeeAsync(int employeeId, int companyId);
    Task<bool> ApplyAsync(CreateLeaveDto dto);
    Task<IEnumerable<LeaveBalanceDto>> GetBalanceAsync(int employeeId, int companyId);

    // Admin actions
    Task<IEnumerable<LeaveApplicationDto>> GetAllAsync(int companyId);
    Task<IEnumerable<LeaveApplicationDto>> GetPendingAsync(int companyId);
    Task<bool> ApproveAsync(int id, string actionBy, int companyId);
    Task<bool> RejectAsync(int id, string reason, string actionBy, int companyId);

    // Leave types (global — not company-scoped)
    Task<IEnumerable<LeaveTypeDto>> GetLeaveTypesAsync();
    Task<bool> CreateLeaveTypeAsync(string name, int days, string? description);
    Task<bool> DeleteLeaveTypeAsync(int id);
}