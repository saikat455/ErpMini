// ErpMini.Domain/Entities/Department.cs
namespace ErpMini.Domain.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
}