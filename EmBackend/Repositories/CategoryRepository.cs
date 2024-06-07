using EmBackend.Entities;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using MongoDB.Driver;

namespace EmBackend.Repositories;

public class CategoryRepository: IRepository<Category>
{
    private readonly IMongoCollection<Category>? _categoriesCollection;
    
    public CategoryRepository(MongoDbService mongoDbService)
    {
        _categoriesCollection = mongoDbService.Database?.GetCollection<Category>("categories");
    }

    public async Task<Category?> Create(Category category)
    {
        var insert = _categoriesCollection?.InsertOneAsync(category);
        if (insert == null) { return null; }
        
        await insert;
        
        return category;
    }

    public async Task<Category?> Update(UpdateDefinition<Category> update, FilterDefinition<Category> filter)
    {
        var updateTask = _categoriesCollection?.FindOneAndUpdateAsync(filter, update);
        if (updateTask == null) { return null; }
        
        return await updateTask;
    }

    public async Task<Category?> GetOne(FilterDefinition<Category> filter)
    {
        var categories = await GetAll(filter);
        return categories?.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        if (_categoriesCollection == null) { return []; }
        
        var categories = _categoriesCollection.Find(_ => true)?.ToListAsync();
        if (categories == null) { return []; }
        
        return await categories;
    }

    public async Task<IEnumerable<Category>> GetAll(FilterDefinition<Category> filter)
    {
        if (_categoriesCollection == null) { return []; }
        
        var categories = _categoriesCollection.Find(filter)?.ToListAsync();
        if (categories == null) { return []; }
        
        return await categories;
    }

    public async Task<DeleteResult?> Delete(FilterDefinition<Category> filter)
    {
        var deleteTask = _categoriesCollection?.DeleteOneAsync(filter);
        if (deleteTask == null) { return null; }
        
        return await deleteTask;
    }
}