// ErpMini.Web/ViewModels/ProcurementViewModel.cs
using ErpMini.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ErpMini.Web.ViewModels;

public class VendorViewModel
{
    public int Id { get; set; }

    [Required] public string Name { get; set; } = string.Empty;
    [Required] [Display(Name = "Contact Person")]
    public string ContactPerson { get; set; } = string.Empty;
    [Required] [EmailAddress] public string Email { get; set; } = string.Empty;
    [Required] public string Phone { get; set; } = string.Empty;
    public string? Address { get; set; }
}

public class CreatePurchaseOrderViewModel
{
    [Required] [Display(Name = "Vendor")]
    public int VendorId { get; set; }

    [Required] [Display(Name = "Order Date")]
    public DateTime OrderDate { get; set; } = DateTime.Today;

    [Display(Name = "Expected Delivery")]
    public DateTime? ExpectedDate { get; set; }

    public string? Notes { get; set; }

    public List<SelectListItem> Vendors { get; set; } = new();

    // Items entered via JS dynamic rows
    public List<OrderItemRow> Items { get; set; } = new();
}

public class OrderItemRow
{
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
}