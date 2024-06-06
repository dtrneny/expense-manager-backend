using EmBackend.Entities;
using EmBackend.Services;
using EmBackend.Utilities;
using MongoDB.Driver;

namespace EmBackend.Repositories;

public class AuthRepository
{
    private readonly IMongoCollection<RefreshToken>? _refreshTokenCollection;
    public readonly JwtService JwtService;
    
    public AuthRepository(MongoDbService mongoDbService, JwtService jwtService)
    {
        _refreshTokenCollection = mongoDbService.Database?.GetCollection<RefreshToken>("refreshTokens");;
        JwtService = jwtService;
        
        SetupExpiration();
    }
    
    private void SetupExpiration()
    {
        var indexBuilder = Builders<RefreshToken>.IndexKeys;
        
        if (indexBuilder == null || _refreshTokenCollection?.Indexes == null) { return; }
        
        var expirationModel = new CreateIndexModel<RefreshToken>(
            keys: indexBuilder.Ascending("Expires"),
            options: new CreateIndexOptions
            {
                ExpireAfter = TimeSpan.FromSeconds(0),
                Name = "ExpireAtIndex"
            }
        );
        
        _refreshTokenCollection.Indexes.CreateOne(expirationModel);
    }

    public async Task<(string accessToken, string refreshToken)?> GetJwtTokens(User user)
    {
        if (user.Id == null) { return null; }
        
        var accessToken = JwtService.GenerateAccessToken(user.Id);
        var refreshTokenString = JwtService.GenerateRefreshToken();
        var refreshToken = await CreateRefreshToken(refreshTokenString, user.Id, accessToken);

        if (refreshToken == null) { return null;}
        
        return (accessToken, refreshTokenString);
    }

    public async Task<string?> RefreshAccessToken(FilterDefinition<RefreshToken> filter)
    {
        var tokens = await GetAll(filter);
        var token = tokens?.FirstOrDefault();

        if (token == null) { return null; }
        var accessToken = JwtService.GenerateAccessToken(token.UserId);
        
        var changesDocument = BsonUtilities.ToBsonDocument(new { AccessToken = accessToken });
        var update = EntityOperationBuilder<RefreshToken>.BuildUpdateDefinition(changesDocument);
        if (update == null) { return null; }

        var updateResult = await Update(update, filter);
        if (updateResult == null) { return null; }
        
        return accessToken;
    }
    
    public async Task<RefreshToken?> Update(UpdateDefinition<RefreshToken> update, FilterDefinition<RefreshToken> filter)
    {
        var updateTask = _refreshTokenCollection?.FindOneAndUpdateAsync(filter, update);
        if (updateTask == null) { return null; }
      
        var updateResult = await updateTask;
        
        return updateResult;
    }
    
    public async Task<RefreshToken?> CreateRefreshToken(string token, string userId, string accessToken)
    {
        var refreshToken = new RefreshToken()
        {
            Token = token,
            UserId = userId,
            Expires = DateTime.Now.AddDays(7),
            AccessToken = accessToken,
        };
        
        var insert = _refreshTokenCollection?.InsertOneAsync(refreshToken);
        if (insert == null) { return null; }
        
        await insert;
        
        return refreshToken;
    }
    
    public async Task<IEnumerable<RefreshToken>> GetAll(FilterDefinition<RefreshToken> filter)
    {
        if (_refreshTokenCollection == null) { return []; }
        
        var tokens = _refreshTokenCollection.Find(filter)?.ToListAsync();
    
        if (tokens == null) { return []; }
        
        return await tokens;
    }
    
    public async Task<DeleteResult?> DeleteRefreshToken(FilterDefinition<RefreshToken> filter)
    {
        var deleteTask = _refreshTokenCollection?.DeleteOneAsync(filter);
        if (deleteTask == null) { return null; }
        
        return await deleteTask;
    }
}