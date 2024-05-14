using EmBackend.Entities;
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
}