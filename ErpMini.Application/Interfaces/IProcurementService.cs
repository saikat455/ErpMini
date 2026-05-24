// ErpMini.Application/Interfaces/IProcurementService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IProcurementService
{
    // Vendors
    Task<IEnumerable<VendorDto>> GetAllVendorsAsync();
    Task<VendorDto?> GetVendorByIdAsync(int id);
    Task<bool> CreateVendorAsync(VendorDto dto);
    Task<bool> UpdateVendorAsync(VendorDto dto);
    Task<bool> DeleteVendorAsync(int id);

    // Purchase Orders
    Task<IEnumerable<PurchaseOrderDto>> GetAllOrdersAsync();
    Task<PurchaseOrderDto?> GetOrderByIdAsync(int id);
    Task<bool> CreateOrderAsync(CreatePurchaseOrderDto dto);
    Task<bool> SubmitOrderAsync(int id);
    Task<bool> ApproveOrderAsync(int id, string approvedBy);
    Task<bool> ReceiveOrderAsync(int id);
    Task<bool> CancelOrderAsync(int id);
    Task<bool> DeleteOrderAsync(int id);
    Task<string> GeneratePoNumberAsync();
}