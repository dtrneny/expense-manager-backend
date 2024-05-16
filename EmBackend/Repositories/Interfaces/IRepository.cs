using MongoDB.Driver;

namespace EmBackend.Repositories.Interfaces;

public interface IRepository<T>
{
    public Task<T?> Create(T item);
    public Task<T?> Update(UpdateDefinition<T> update, FilterDefinition<T> filter);
    public Task<T?> GetOne(FilterDefinition<T> filter);
    public Task<IEnumerable<T>> GetAll();
    public Task<IEnumerable<T>> GetAll(FilterDefinition<T> filter);
    public Task<DeleteResult?> Delete(FilterDefinition<T> filter);
}