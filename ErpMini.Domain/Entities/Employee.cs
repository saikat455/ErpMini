// ErpMini.Domain/Entities/Employee.cs
namespace ErpMini.Domain.Entities;

public class Employee : BaseEntity
{
    public string EmployeeCode { get; set; } = string.Empty;  // e.g. EMP-001
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime JoiningDate { get; set; }
    public decimal BasicSalary { get; set; }
    public bool IsActive { get; set; } = true;
    public string Gender { get; set; } = string.Empty;

    // Foreign Keys
    public int DepartmentId { get; set; }
    public int DesignationId { get; set; }
    public string? UserId { get; set; }  // link to ApplicationUser

    // Navigation
    public Department Department { get; set; } = null!;
    public Designation Designation { get; set; } = null!;
}