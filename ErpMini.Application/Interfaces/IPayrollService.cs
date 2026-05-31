// ErpMini.Application/Interfaces/IPayrollService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IPayrollService
{
    Task<IEnumerable<PayrollDto>> GetAllAsync(int companyId);
    Task<IEnumerable<PayrollDto>> GetByMonthYearAsync(int month, int year, int companyId);
    Task<PayrollDto?> GetByIdAsync(int id, int companyId);
    Task<bool> GenerateAsync(GeneratePayrollDto dto);
    Task<bool> GenerateBulkAsync(int month, int year, string generatedBy, int companyId);
    Task<bool> MarkAsPaidAsync(int id, int companyId);
    Task<bool> DeleteAsync(int id, int companyId);
    Task<bool> ExistsAsync(int employeeId, int month, int year);
    Task<IEnumerable<PayrollSummaryDto>> GetSummaryAsync(int companyId);
}