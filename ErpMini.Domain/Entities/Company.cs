// ErpMini.Domain/Entities/Company.cs
namespace ErpMini.Domain.Entities;

public class Company : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string CompanyCode { get; set; } = string.Empty;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    public ICollection<Department> Departments { get; set; } = new List<Department>();
    public ICollection<Designation> Designations { get; set; } = new List<Designation>();
}