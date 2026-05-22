// ErpMini.Infrastructure/UnitOfWork.cs
using ErpMini.Application.Interfaces;
using ErpMini.Infrastructure.Data;

namespace ErpMini.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}