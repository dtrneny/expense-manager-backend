using MongoDB.Driver;

namespace EmBackend.Repositories;

public interface IRepository<T>
{
    public Task<T?> Create(T item);
    // public Task<T?> Update(T item);
    // public Task<T?> GetById(string id);
    public Task<IEnumerable<T>> GetAll();
    public Task<IEnumerable<T>> GetAll(FilterDefinition<T> filter);
}