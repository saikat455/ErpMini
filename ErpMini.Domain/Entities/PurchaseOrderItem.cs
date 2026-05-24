// ErpMini.Domain/Entities/PurchaseOrderItem.cs
namespace ErpMini.Domain.Entities;

public class PurchaseOrderItem : BaseEntity
{
    public int PurchaseOrderId { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice => Quantity * UnitPrice;
    public bool IsReceived { get; set; } = false;

    // Navigation
    public PurchaseOrder PurchaseOrder { get; set; } = null!;
}