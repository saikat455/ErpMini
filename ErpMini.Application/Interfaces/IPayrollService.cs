// ErpMini.Application/Interfaces/IPayrollService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IPayrollService
{
    Task<IEnumerable<PayrollDto>> GetAllAsync();
    Task<IEnumerable<PayrollDto>> GetByMonthYearAsync(int month, int year);
    Task<PayrollDto?> GetByIdAsync(int id);
    Task<bool> GenerateAsync(GeneratePayrollDto dto);
    Task<bool> GenerateBulkAsync(int month, int year, string generatedBy);
    Task<bool> MarkAsPaidAsync(int id);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int employeeId, int month, int year);
    Task<IEnumerable<PayrollSummaryDto>> GetSummaryAsync();
}