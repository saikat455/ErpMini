// ErpMini.Application/Interfaces/IEmployeeService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync();
    Task<EmployeeDto?> GetByIdAsync(int id);
    Task<bool> CreateAsync(CreateEmployeeDto dto);
    Task<bool> UpdateAsync(int id, CreateEmployeeDto dto);
    Task<bool> DeleteAsync(int id);
    Task<string> GenerateEmployeeCodeAsync();
}

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync();
    Task<DepartmentDto?> GetByIdAsync(int id);
    Task<bool> CreateAsync(string name, string? description);
    Task<bool> UpdateAsync(int id, string name, string? description);
    Task<bool> DeleteAsync(int id);
}

public interface IDesignationService
{
    Task<IEnumerable<DesignationDto>> GetAllAsync();
    Task<bool> CreateAsync(string title, string? description);
    Task<bool> UpdateAsync(int id, string title, string? description);
    Task<bool> DeleteAsync(int id);
}