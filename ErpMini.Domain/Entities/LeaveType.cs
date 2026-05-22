// ErpMini.Domain/Entities/LeaveType.cs
namespace ErpMini.Domain.Entities;

public class LeaveType : BaseEntity
{
    public string Name { get; set; } = string.Empty;        // e.g. Annual, Sick, Casual
    public int TotalDays { get; set; }                      // allowed days per year
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}