// ErpMini.Web/Controllers/DashboardController.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ErpMini.Web.Controllers;

[Authorize]
public class DashboardController : Controller
{
    public IActionResult Index() => View();
}