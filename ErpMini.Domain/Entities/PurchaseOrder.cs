// ErpMini.Domain/Entities/PurchaseOrder.cs
using ErpMini.Domain.Enums;

namespace ErpMini.Domain.Entities;

public class PurchaseOrder : BaseEntity
{
    public string PoNumber { get; set; } = string.Empty;   // e.g. PO-001
    public int VendorId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Draft;
    public string? Notes { get; set; }
    public string? ApprovedBy { get; set; }
    public DateTime? ApprovedOn { get; set; }
    public string CreatedByUser { get; set; } = string.Empty;

    // Navigation
    public Vendor Vendor { get; set; } = null!;
    public ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
}