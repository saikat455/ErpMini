// ErpMini.Web/Helpers/UserContext.cs
using ErpMini.Application.Interfaces;
using ErpMini.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace ErpMini.Web.Helpers;

public class UserContext : ICompanyContext
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContext(UserManager<ApplicationUser> userManager,
        IHttpContextAccessor httpContextAccessor)
    {
        _userManager = userManager;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        return await _userManager.GetUserAsync(
            _httpContextAccessor.HttpContext!.User);
    }

    public async Task<int?> GetCompanyIdAsync()
    {
        var user = await GetCurrentUserAsync();
        return user?.CompanyId;
    }

    public async Task<int> GetCompanyIdRequiredAsync()
    {
        var id = await GetCompanyIdAsync();
        return id ?? throw new InvalidOperationException("User has no company.");
    }
}