using EmBackend.Entities;
using EmBackend.Models.Users.Requests;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Services.HashService;
using MongoDB.Bson;
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

    public async Task<UpdateResult?> Update(UpdateDefinition<User> update, FilterDefinition<User> filter)
    {
        var updateResult = _usersCollection?.UpdateOneAsync(filter, update);
        
        if (updateResult == null) { return null; }
        
        return await updateResult;
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
}