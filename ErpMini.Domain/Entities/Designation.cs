// ErpMini.Domain/Entities/Designation.cs
namespace ErpMini.Domain.Entities;

public class Designation : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;
    public int CompanyId { get; set; }          // ← NEW

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public Company Company { get; set; } = null!; // ← NEW
}