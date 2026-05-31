// ErpMini.Application/Interfaces/IDashboardService.cs
using ErpMini.Application.DTOs;

namespace ErpMini.Application.Interfaces;

public interface IDashboardService
{
    Task<DashboardDto> GetDashboardDataAsync(int companyId);
}