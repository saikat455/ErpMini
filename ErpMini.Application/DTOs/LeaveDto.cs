// ErpMini.Application/DTOs/LeaveDto.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Application.DTOs;

public class LeaveApplicationDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string EmployeeCode { get; set; } = string.Empty;
    public string DepartmentName { get; set; } = string.Empty;
    public int LeaveTypeId { get; set; }
    public string LeaveTypeName { get; set; } = string.Empty;
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; }
    public string StatusLabel => Status.ToString();
    public string? RejectionReason { get; set; }
    public DateTime? ActionDate { get; set; }
    public string? ActionBy { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateLeaveDto
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class LeaveTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int TotalDays { get; set; }
    public bool IsActive { get; set; }
}

public class LeaveBalanceDto
{
    public string LeaveTypeName { get; set; } = string.Empty;
    public int Allocated { get; set; }
    public int Used { get; set; }
    public int Remaining => Allocated - Used;
}