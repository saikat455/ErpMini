// ErpMini.Web/ViewModels/EmployeeViewModel.cs
using ErpMini.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ErpMini.Web.ViewModels;

public class EmployeeViewModel
{
    public int Id { get; set; }

    [Required] [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required] [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required] [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Phone { get; set; } = string.Empty;

    public string? Address { get; set; }

    [Required] [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [Required] [Display(Name = "Joining Date")]
    public DateTime JoiningDate { get; set; }

    [Required] [Range(1, double.MaxValue, ErrorMessage = "Salary must be greater than 0")]
    public decimal BasicSalary { get; set; }

    [Required]
    public string Gender { get; set; } = string.Empty;

    [Required] [Display(Name = "Department")]
    public int DepartmentId { get; set; }

    [Required] [Display(Name = "Designation")]
    public int DesignationId { get; set; }

    // For dropdowns
    public List<SelectListItem> Departments { get; set; } = new();
    public List<SelectListItem> Designations { get; set; } = new();
}