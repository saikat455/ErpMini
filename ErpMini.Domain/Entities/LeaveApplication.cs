// ErpMini.Domain/Entities/LeaveApplication.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Domain.Entities;

public class LeaveApplication : BaseEntity
{
    public int EmployeeId { get; set; }
    public int LeaveTypeId { get; set; }
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public int TotalDays { get; set; }
    public string Reason { get; set; } = string.Empty;
    public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
    public string? RejectionReason { get; set; }
    public DateTime? ActionDate { get; set; }       // when approved/rejected
    public string? ActionBy { get; set; }           // who approved/rejected

    // Navigation
    public Employee Employee { get; set; } = null!;
    public LeaveType LeaveType { get; set; } = null!;
}