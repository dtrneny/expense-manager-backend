using EmBackend.Entities;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Services.HashService;
using MongoDB.Driver;

namespace EmBackend.Repositories;

public class MovementRepository: IRepository<Movement>
{
    private readonly IMongoCollection<Movement>? _movementsCollection;
    
    public MovementRepository(MongoDbService mongoDbService, IHashService hashService)
    {
        _movementsCollection = mongoDbService.Database?.GetCollection<Movement>("movements");
    }

    public async Task<Movement?> Create(Movement movement)
    {
        var insert = _movementsCollection?.InsertOneAsync(movement);
        if (insert == null) { return null; }
        
        await insert;
        
        return movement;
    }

    public async Task<Movement?> Update(UpdateDefinition<Movement> update, FilterDefinition<Movement> filter)
    {
        var updateTask = _movementsCollection?.FindOneAndUpdateAsync(filter, update);
        if (updateTask == null) { return null; }
      
        var updateResult = await updateTask;
        
        return updateResult;
    }

    public async Task<Movement?> GetOne(FilterDefinition<Movement> filter)
    {
        var movements = await GetAll(filter);
        return movements?.FirstOrDefault();
    }

    public async Task<IEnumerable<Movement>> GetAll()
    {
        if (_movementsCollection == null) { return []; }
        
        var movements = _movementsCollection.Find(_ => true)?.ToListAsync();
    
        if (movements == null) { return []; }
        
        return await movements;
    }

    public async Task<IEnumerable<Movement>> GetAll(FilterDefinition<Movement> filter)
    {
        if (_movementsCollection == null) { return []; }
        
        var movements = _movementsCollection.Find(filter)?.ToListAsync();
    
        if (movements == null) { return []; }
        
        return await movements;
    }
    
    public async Task<DeleteResult?> Delete(FilterDefinition<Movement> filter)
    {
        var deleteTask = _movementsCollection?.DeleteOneAsync(filter);
        if (deleteTask == null) { return null; }
        
        return await deleteTask;
    }
}