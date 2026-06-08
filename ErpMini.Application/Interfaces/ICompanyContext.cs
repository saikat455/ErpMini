namespace ErpMini.Application.Interfaces;

public interface ICompanyContext
{
    Task<int?> GetCompanyIdAsync();
}
