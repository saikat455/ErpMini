// ErpMini.Infrastructure/Repositories/GenericRepository.cs
using ErpMini.Application.Interfaces;
using ErpMini.Domain.Entities;
using ErpMini.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ErpMini.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public GenericRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(int id)
        => await _dbSet.FirstOrDefaultAsync(e => e.Id == id);

    public async Task<IEnumerable<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public async Task AddAsync(T entity)
        => await _dbSet.AddAsync(entity);

    public void Update(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public void Delete(T entity)
    {
        entity.IsDeleted = true;   // soft delete
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
    }
}