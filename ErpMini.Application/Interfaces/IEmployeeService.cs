// ErpMini.Application/Interfaces/IEmployeeService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeDto>> GetAllAsync(int companyId);
    Task<EmployeeDto?> GetByIdAsync(int id, int companyId);
    Task<bool> CreateAsync(CreateEmployeeDto dto);         // dto carries CompanyId
    Task<bool> UpdateAsync(int id, CreateEmployeeDto dto); // dto carries CompanyId
    Task<bool> UpdatePersonalInfoAsync(int id, UpdatePersonalInfoDto dto);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<string> GenerateEmployeeCodeAsync(int companyId);
}

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllAsync(int companyId);
    Task<DepartmentDto?> GetByIdAsync(int id, int companyId);
    Task<bool> CreateAsync(string name, string? description, int companyId);
    Task<bool> UpdateAsync(int id, string name, string? description, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
}

public interface IDesignationService
{
    Task<IEnumerable<DesignationDto>> GetAllAsync(int companyId);
    Task<bool> CreateAsync(string title, string? description, int companyId);
    Task<bool> UpdateAsync(int id, string title, string? description, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
}