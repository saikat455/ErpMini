// ErpMini.Application/Interfaces/IUnitOfWork.cs
namespace ErpMini.Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    // IGenericRepository<Employee> Employees { get; }
    // IGenericRepository<Department> Departments { get; }
    Task<int> SaveChangesAsync();
}