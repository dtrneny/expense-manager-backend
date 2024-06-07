using EmBackend.Entities;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Services.Interfaces;
using MongoDB.Driver;

namespace EmBackend.Repositories;

public class UserRepository: IRepository<User>
{
    private readonly IMongoCollection<User>? _usersCollection;
    private readonly IHashService _hashService;
    
    public UserRepository(MongoDbService mongoDbService, IHashService hashService)
    {
        _usersCollection = mongoDbService.Database?.GetCollection<User>("users");
        _hashService = hashService;
    }
    
    public async Task<User?> Create(User user)
    {
        var hashedPassword = _hashService.Hash(user.Password);
        if (hashedPassword == null) { return null; }

        user.Password = hashedPassword;
        
        var insert = _usersCollection?.InsertOneAsync(user);
        if (insert == null) { return null; }
        
        await insert;
        
        return user;
    }

    public async Task<User?> Update(UpdateDefinition<User> update, FilterDefinition<User> filter)
    {
        var updateTask = _usersCollection?.FindOneAndUpdateAsync(filter, update);
        if (updateTask == null) { return null; }
        
        return await updateTask;
    }

    public async Task<User?> GetOne(FilterDefinition<User> filter)
    {
        var users = await GetAll(filter);
        return users?.FirstOrDefault();
    }

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
    
    public async Task<DeleteResult?> Delete(FilterDefinition<User> filter)
    {
        var deleteTask = _usersCollection?.DeleteOneAsync(filter);
        if (deleteTask == null) { return null; }
        
        return await deleteTask;
    }
}