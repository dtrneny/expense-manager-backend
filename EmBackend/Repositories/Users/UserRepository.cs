using EmBackend.Entities;
using EmBackend.Services;
using MongoDB.Driver;

namespace EmBackend.Repositories.Users;

public class UserRepository: IRepository<User>
{
    private readonly IMongoCollection<User>? _usersCollection;
    
    public UserRepository(MongoDbService mongoDbService)
    {
        _usersCollection = mongoDbService.Database?.GetCollection<User>("users");
    }
    
    public async Task<User?> Create(User user)
    {
        var insert = _usersCollection?.InsertOneAsync(user);
        if (insert == null) { return null; }
        
        await insert;
        
        return user;
    }

    // public Task<User?> Update(User user)
    // {
    //     throw new NotImplementedException();
    // }
    //
    // public Task<User?> GetById(string id)
    // {
    //     throw new NotImplementedException();
    // }

    public async Task<IEnumerable<User>> GetAll()
    {
        if (_usersCollection == null) { return []; }
        
        var users = _usersCollection.Find(_ => true)?.ToListAsync();
    
        if (users == null) { return []; }
        
        return await users;
    }
    
    public async Task<IEnumerable<User>> GetAll(FilterDefinition<User> filter)
    {
        if (_usersCollection == null) { return []; }
        
        var users = _usersCollection.Find(filter)?.ToListAsync();
    
        if (users == null) { return []; }
        
        return await users;
    }
}