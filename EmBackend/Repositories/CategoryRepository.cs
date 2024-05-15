using EmBackend.Entities;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using MongoDB.Driver;

namespace EmBackend.Repositories;

public class CategoryRepository: IRepository<Category>
{
    private readonly IMongoCollection<Category>? _categoryCollection;
    
    public CategoryRepository(MongoDbService mongoDbService)
    {
        _categoryCollection = mongoDbService.Database?.GetCollection<Category>("categories");
    }

    public async Task<Category?> Create(Category category)
    {
        var insert = _categoryCollection?.InsertOneAsync(category);
        if (insert == null) { return null; }
        
        await insert;
        
        return category;
    }

    public Task<UpdateResult?> Update(UpdateDefinition<Category> update, FilterDefinition<Category> filter)
    {
        throw new NotImplementedException();
    }

    public async Task<Category?> GetOne(FilterDefinition<Category> filter)
    {
        var categories = await GetAll(filter);
        return categories?.FirstOrDefault();
    }

    public async Task<IEnumerable<Category>> GetAll()
    {
        if (_categoryCollection == null) { return []; }
        
        var categories = _categoryCollection.Find(_ => true)?.ToListAsync();
    
        if (categories == null) { return []; }
        
        return await categories;
    }

    public async Task<IEnumerable<Category>> GetAll(FilterDefinition<Category> filter)
    {
        if (_categoryCollection == null) { return []; }
        
        var categories = _categoryCollection.Find(filter)?.ToListAsync();
    
        if (categories == null) { return []; }
        
        return await categories;
    }
}