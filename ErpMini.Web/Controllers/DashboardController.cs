// ErpMini.Web/Controllers/DashboardController.cs
using ErpMini.Application.Interfaces;
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DashboardController : BaseController
{
    private readonly IDashboardService _dashboardService;

    public DashboardController(
        IDashboardService dashboardService,
        UserContext userContext) : base(userContext)
    {
        _dashboardService = dashboardService;
    }

    public async Task<IActionResult> Index()
    {
        var companyId = await GetCompanyIdAsync();
        var data = await _dashboardService.GetDashboardDataAsync(companyId);
        return View(data);
    }
}