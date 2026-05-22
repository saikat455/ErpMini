// ErpMini.Infrastructure/Services/DesignationService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class DesignationService : IDesignationService
{
    private readonly AppDbContext _context;
    public DesignationService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<DesignationDto>> GetAllAsync()
    {
        return await _context.Designations
            .Select(d => new DesignationDto
            {
                Id = d.Id,
                Title = d.Title,
                Description = d.Description,
                IsActive = d.IsActive
            }).ToListAsync();
    }

    public async Task<bool> CreateAsync(string title, string? description)
    {
        await _context.Designations.AddAsync(new Designation { Title = title, Description = description, CreatedAt = DateTime.UtcNow });
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(int id, string title, string? description)
    {
        var d = await _context.Designations.FindAsync(id);
        if (d is null) return false;
        d.Title = title; d.Description = description; d.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var d = await _context.Designations.FindAsync(id);
        if (d is null) return false;
        d.IsDeleted = true; d.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }
}