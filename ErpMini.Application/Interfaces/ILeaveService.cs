// ErpMini.Application/Interfaces/ILeaveService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface ILeaveService
{
    // Employee actions
    Task<IEnumerable<LeaveApplicationDto>> GetByEmployeeAsync(int employeeId);
    Task<bool> ApplyAsync(CreateLeaveDto dto);
    Task<IEnumerable<LeaveBalanceDto>> GetBalanceAsync(int employeeId);

    // Admin actions
    Task<IEnumerable<LeaveApplicationDto>> GetAllAsync();
    Task<IEnumerable<LeaveApplicationDto>> GetPendingAsync();
    Task<bool> ApproveAsync(int id, string actionBy);
    Task<bool> RejectAsync(int id, string reason, string actionBy);

    // Leave types
    Task<IEnumerable<LeaveTypeDto>> GetLeaveTypesAsync();
    Task<bool> CreateLeaveTypeAsync(string name, int days, string? description);
    Task<bool> DeleteLeaveTypeAsync(int id);
}