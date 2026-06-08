using ErpMini.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ErpMini.Infrastructure.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionsBuilder.UseNpgsql(
            "Server=localhost;Database=ErpMini;User Id=postgres;Password=postgres;");

        return new AppDbContext(optionsBuilder.Options, new DesignTimeCompanyContext());
    }

    private class DesignTimeCompanyContext : ICompanyContext
    {
        public Task<int?> GetCompanyIdAsync() => Task.FromResult<int?>(null);
    }
}
