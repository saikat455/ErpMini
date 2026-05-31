// ErpMini.Infrastructure/Services/DepartmentService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class DepartmentService : IDepartmentService
{
    private readonly AppDbContext _context;
    public DepartmentService(AppDbContext context) => _context = context;

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(int companyId)
    {
        return await _context.Departments
            .Where(d => d.CompanyId == companyId)
            .Select(d => new DepartmentDto
            {
                Id            = d.Id,
                Name          = d.Name,
                Description   = d.Description,
                IsActive      = d.IsActive,
                EmployeeCount = d.Employees.Count(e => !e.IsDeleted)
            })
            .ToListAsync();
    }

    public async Task<DepartmentDto?> GetByIdAsync(int id, int companyId)
    {
        var d = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == companyId);

        if (d is null) return null;

        return new DepartmentDto
        {
            Id          = d.Id,
            Name        = d.Name,
            Description = d.Description,
            IsActive    = d.IsActive
        };
    }

    public async Task<bool> CreateAsync(string name, string? description, int companyId)
    {
        var dept = new Department
        {
            Name        = name,
            Description = description,
            CompanyId   = companyId,
            CreatedAt   = DateTime.UtcNow
        };
        await _context.Departments.AddAsync(dept);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(int id, string name, string? description, int companyId)
    {
        var dept = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == companyId);

        if (dept is null) return false;

        dept.Name        = name;
        dept.Description = description;
        dept.UpdatedAt   = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var dept = await _context.Departments
            .FirstOrDefaultAsync(d => d.Id == id && d.CompanyId == companyId);

        if (dept is null) return false;

        dept.IsDeleted = true;
        dept.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }
}