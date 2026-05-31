// ErpMini.Infrastructure/Services/ProcurementService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Domain.Enums;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class ProcurementService : IProcurementService
{
    private readonly AppDbContext _context;
    public ProcurementService(AppDbContext context) => _context = context;

    // ── Vendors ────────────────────────────────────────────────────────────

    public async Task<IEnumerable<VendorDto>> GetAllVendorsAsync(int companyId)
    {
        return await _context.Vendors
            .Where(v => v.CompanyId == companyId)
            .Select(v => new VendorDto
            {
                Id            = v.Id,
                Name          = v.Name,
                ContactPerson = v.ContactPerson,
                Email         = v.Email,
                Phone         = v.Phone,
                Address       = v.Address,
                IsActive      = v.IsActive,
                TotalOrders   = v.PurchaseOrders.Count(p => !p.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<VendorDto?> GetVendorByIdAsync(int id, int companyId)
    {
        var v = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

        if (v is null) return null;

        return new VendorDto
        {
            Id            = v.Id,
            Name          = v.Name,
            ContactPerson = v.ContactPerson,
            Email         = v.Email,
            Phone         = v.Phone,
            Address       = v.Address,
            IsActive      = v.IsActive
        };
    }

    public async Task<bool> CreateVendorAsync(VendorDto dto, int companyId)
    {
        var vendor = new Vendor
        {
            Name          = dto.Name,
            ContactPerson = dto.ContactPerson,
            Email         = dto.Email,
            Phone         = dto.Phone,
            Address       = dto.Address,
            CompanyId     = companyId,
            CreatedAt     = DateTime.UtcNow
        };
        await _context.Vendors.AddAsync(vendor);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateVendorAsync(VendorDto dto, int companyId)
    {
        var v = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == dto.Id && v.CompanyId == companyId);

        if (v is null) return false;

        v.Name          = dto.Name;
        v.ContactPerson = dto.ContactPerson;
        v.Email         = dto.Email;
        v.Phone         = dto.Phone;
        v.Address       = dto.Address;
        v.UpdatedAt     = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteVendorAsync(int id, int companyId)
    {
        var v = await _context.Vendors
            .FirstOrDefaultAsync(v => v.Id == id && v.CompanyId == companyId);

        if (v is null) return false;

        v.IsDeleted = true;
        v.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    // ── Purchase Orders ────────────────────────────────────────────────────

    public async Task<IEnumerable<PurchaseOrderDto>> GetAllOrdersAsync(int companyId)
    {
        return await _context.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Items)
            .Where(p => p.CompanyId == companyId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => MapToDto(p))
            .ToListAsync();
    }

    public async Task<PurchaseOrderDto?> GetOrderByIdAsync(int id, int companyId)
    {
        var p = await _context.PurchaseOrders
            .Include(p => p.Vendor)
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

        return p is null ? null : MapToDto(p);
    }

    public async Task<bool> CreateOrderAsync(CreatePurchaseOrderDto dto)
    {
        var order = new PurchaseOrder
        {
            PoNumber     = await GeneratePoNumberAsync(dto.CompanyId),
            VendorId     = dto.VendorId,
            CompanyId    = dto.CompanyId,
            OrderDate    = DateTime.SpecifyKind(dto.OrderDate, DateTimeKind.Utc),
            ExpectedDate = dto.ExpectedDate.HasValue
                ? DateTime.SpecifyKind(dto.ExpectedDate.Value, DateTimeKind.Utc)
                : null,
            Notes         = dto.Notes,
            Status        = PurchaseOrderStatus.Draft,
            CreatedByUser = dto.CreatedByUser,
            CreatedAt     = DateTime.UtcNow,
            Items         = dto.Items.Select(i => new PurchaseOrderItem
            {
                ItemName    = i.ItemName,
                Description = i.Description,
                Quantity    = i.Quantity,
                UnitPrice   = i.UnitPrice,
                CreatedAt   = DateTime.UtcNow
            }).ToList()
        };

        await _context.PurchaseOrders.AddAsync(order);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> SubmitOrderAsync(int id, int companyId)
        => await UpdateStatus(id, companyId, PurchaseOrderStatus.Submitted);

    public async Task<bool> ApproveOrderAsync(int id, string approvedBy, int companyId)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

        if (order is null) return false;

        order.Status     = PurchaseOrderStatus.Approved;
        order.ApprovedBy = approvedBy;
        order.ApprovedOn = DateTime.UtcNow;
        order.UpdatedAt  = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ReceiveOrderAsync(int id, int companyId)
    {
        var order = await _context.PurchaseOrders
            .Include(p => p.Items)
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

        if (order is null) return false;

        order.Status    = PurchaseOrderStatus.Received;
        order.UpdatedAt = DateTime.UtcNow;

        foreach (var item in order.Items)
            item.IsReceived = true;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> CancelOrderAsync(int id, int companyId)
        => await UpdateStatus(id, companyId, PurchaseOrderStatus.Cancelled);

    public async Task<bool> DeleteOrderAsync(int id, int companyId)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

        if (order is null) return false;

        order.IsDeleted = true;
        order.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<string> GeneratePoNumberAsync(int companyId)
    {
        // Scoped per company — each company has its own PO-001 sequence
        var last = await _context.PurchaseOrders
            .IgnoreQueryFilters()
            .Where(p => p.CompanyId == companyId)
            .OrderByDescending(p => p.Id)
            .Select(p => p.PoNumber)
            .FirstOrDefaultAsync();

        if (last is null) return "PO-001";
        var num = int.Parse(last.Split('-')[1]);
        return $"PO-{(num + 1):D3}";
    }

    // ── Private helpers ────────────────────────────────────────────────────

    private async Task<bool> UpdateStatus(
        int id, int companyId, PurchaseOrderStatus status)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(p => p.Id == id && p.CompanyId == companyId);

        if (order is null) return false;

        order.Status    = status;
        order.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    private static PurchaseOrderDto MapToDto(PurchaseOrder p) => new()
    {
        Id            = p.Id,
        PoNumber      = p.PoNumber,
        VendorId      = p.VendorId,
        VendorName    = p.Vendor.Name,
        OrderDate     = p.OrderDate,
        ExpectedDate  = p.ExpectedDate,
        Status        = p.Status,
        Notes         = p.Notes,
        ApprovedBy    = p.ApprovedBy,
        ApprovedOn    = p.ApprovedOn,
        CreatedByUser = p.CreatedByUser,
        CreatedAt     = p.CreatedAt,
        Items         = p.Items.Select(i => new PurchaseOrderItemDto
        {
            Id          = i.Id,
            ItemName    = i.ItemName,
            Description = i.Description,
            Quantity    = i.Quantity,
            UnitPrice   = i.UnitPrice,
            IsReceived  = i.IsReceived
        }).ToList()
    };
}