// ErpMini.Web/ViewModels/RegisterViewModel.cs
namespace ErpMini.Web.ViewModels;

public class RegisterViewModel
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string AccountType { get; set; } = "Employee";
    public string? CompanyName { get; set; }
    public string? CompanyCode { get; set; }
}