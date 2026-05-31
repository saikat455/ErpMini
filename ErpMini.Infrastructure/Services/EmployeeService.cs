// ErpMini.Infrastructure/Services/EmployeeService.cs
using ErpMini.Application.DTOs;
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Services;

public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;

    public EmployeeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmployeeDto>> GetAllAsync(int companyId)
    {
        return await _context.Employees
            .Where(e => e.CompanyId == companyId)
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Select(e => new EmployeeDto
            {
                Id               = e.Id,
                EmployeeCode     = e.EmployeeCode,
                FirstName        = e.FirstName,
                LastName         = e.LastName,
                Email            = e.Email,
                Phone            = e.Phone,
                Address          = e.Address,
                DateOfBirth      = e.DateOfBirth,
                JoiningDate      = e.JoiningDate,
                BasicSalary      = e.BasicSalary,
                IsActive         = e.IsActive,
                Gender           = e.Gender,
                DepartmentId     = e.DepartmentId,
                DepartmentName   = e.Department.Name,
                DesignationId    = e.DesignationId,
                DesignationTitle = e.Designation.Title
            })
            .ToListAsync();
    }

    public async Task<EmployeeDto?> GetByIdAsync(int id, int companyId)
    {
        var e = await _context.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId);

        if (e is null) return null;

        return new EmployeeDto
        {
            Id               = e.Id,
            EmployeeCode     = e.EmployeeCode,
            FirstName        = e.FirstName,
            LastName         = e.LastName,
            Email            = e.Email,
            Phone            = e.Phone,
            Address          = e.Address,
            DateOfBirth      = e.DateOfBirth,
            JoiningDate      = e.JoiningDate,
            BasicSalary      = e.BasicSalary,
            IsActive         = e.IsActive,
            Gender           = e.Gender,
            DepartmentId     = e.DepartmentId,
            DepartmentName   = e.Department.Name,
            DesignationId    = e.DesignationId,
            DesignationTitle = e.Designation.Title
        };
    }

    public async Task<bool> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee
        {
            EmployeeCode  = await GenerateEmployeeCodeAsync(dto.CompanyId),
            FirstName     = dto.FirstName,
            LastName      = dto.LastName,
            Email         = dto.Email,
            Phone         = dto.Phone,
            Address       = dto.Address,
            DateOfBirth   = DateTime.SpecifyKind(dto.DateOfBirth, DateTimeKind.Utc),
            JoiningDate   = DateTime.SpecifyKind(dto.JoiningDate, DateTimeKind.Utc),
            BasicSalary   = dto.BasicSalary,
            Gender        = dto.Gender,
            DepartmentId  = dto.DepartmentId,
            DesignationId = dto.DesignationId,
            CompanyId     = dto.CompanyId,
            CreatedAt     = DateTime.UtcNow
        };

        await _context.Employees.AddAsync(employee);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> UpdateAsync(int id, CreateEmployeeDto dto)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == dto.CompanyId);

        if (employee is null) return false;

        employee.FirstName     = dto.FirstName;
        employee.LastName      = dto.LastName;
        employee.Email         = dto.Email;
        employee.Phone         = dto.Phone;
        employee.Address       = dto.Address;
        employee.DateOfBirth   = DateTime.SpecifyKind(dto.DateOfBirth, DateTimeKind.Utc);
        employee.JoiningDate   = DateTime.SpecifyKind(dto.JoiningDate, DateTimeKind.Utc);
        employee.BasicSalary   = dto.BasicSalary;
        employee.Gender        = dto.Gender;
        employee.DepartmentId  = dto.DepartmentId;
        employee.DesignationId = dto.DesignationId;
        employee.UpdatedAt     = DateTime.UtcNow;

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id, int companyId)
    {
        var employee = await _context.Employees
            .FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId);

        if (employee is null) return false;

        employee.IsDeleted = true;
        employee.UpdatedAt = DateTime.UtcNow;
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<string> GenerateEmployeeCodeAsync(int companyId)
    {
        // Scoped per company — each company has its own EMP-001 sequence
        var lastCode = await _context.Employees
            .IgnoreQueryFilters()
            .Where(e => e.CompanyId == companyId)
            .OrderByDescending(e => e.Id)
            .Select(e => e.EmployeeCode)
            .FirstOrDefaultAsync();

        if (lastCode is null) return "EMP-001";

        var number = int.Parse(lastCode.Split('-')[1]);
        return $"EMP-{(number + 1):D3}";
    }
}