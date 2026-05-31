// ErpMini.Web/Controllers/BaseController.cs
using ErpMini.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

public class BaseController : Controller
{
    protected readonly UserContext UserContext;

    public BaseController(UserContext userContext)
    {
        UserContext = userContext;
    }

    protected async Task<int> GetCompanyIdAsync()
    {
        var id = await UserContext.GetCompanyIdAsync();
        if (id is null)
            throw new InvalidOperationException("No company associated with this user.");
        return id.Value;
    }
}