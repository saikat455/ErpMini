// ErpMini.Application/Interfaces/IProcurementService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IProcurementService
{
    // Vendors
    Task<IEnumerable<VendorDto>> GetAllVendorsAsync(int companyId);
    Task<VendorDto?> GetVendorByIdAsync(int id, int companyId);
    Task<bool> CreateVendorAsync(VendorDto dto, int companyId);
    Task<bool> UpdateVendorAsync(VendorDto dto, int companyId);
    Task<bool> DeleteVendorAsync(int id, int companyId);

    // Purchase Orders
    Task<IEnumerable<PurchaseOrderDto>> GetAllOrdersAsync(int companyId);
    Task<PurchaseOrderDto?> GetOrderByIdAsync(int id, int companyId);
    Task<bool> CreateOrderAsync(CreatePurchaseOrderDto dto);       // dto carries CompanyId
    Task<bool> SubmitOrderAsync(int id, int companyId);
    Task<bool> ApproveOrderAsync(int id, string approvedBy, int companyId);
    Task<bool> ReceiveOrderAsync(int id, int companyId);
    Task<bool> CancelOrderAsync(int id, int companyId);
    Task<bool> DeleteOrderAsync(int id, int companyId);
    Task<string> GeneratePoNumberAsync(int companyId);
}