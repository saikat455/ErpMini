// ErpMini.Web/ViewModels/LeaveViewModel.cs
using ErpMini.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ErpMini.Web.ViewModels;

public class ApplyLeaveViewModel
{
    [Required]
    [Display(Name = "Employee")]
    public int EmployeeId { get; set; }

    [Required]
    [Display(Name = "Leave Type")]
    public int LeaveTypeId { get; set; }

    [Required]
    [Display(Name = "From Date")]
    public DateTime FromDate { get; set; } = DateTime.Today;

    [Required]
    [Display(Name = "To Date")]
    public DateTime ToDate { get; set; } = DateTime.Today;

    [Required]
    [StringLength(500, MinimumLength = 10, ErrorMessage = "Reason must be at least 10 characters.")]
    public string Reason { get; set; } = string.Empty;

    // Dropdowns
    public List<SelectListItem> Employees { get; set; } = new();
    public List<SelectListItem> LeaveTypes { get; set; } = new();
}

public class RejectLeaveViewModel
{
    public int LeaveId { get; set; }
    [Required(ErrorMessage = "Rejection reason is required.")]
    public string Reason { get; set; } = string.Empty;
}