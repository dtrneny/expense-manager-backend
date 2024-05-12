using EmBackend.Entities;
using EmBackend.Services;
using EmBackend.Services.HashService;
using MongoDB.Driver;

namespace EmBackend.Repositories.Auth;

public class AuthRepository
{
    private readonly IMongoCollection<RefreshToken>? _refreshTokenCollection;
    public readonly JwtService JwtService;
    
    public AuthRepository(MongoDbService mongoDbService, JwtService jwtService)
    {
        _refreshTokenCollection = mongoDbService.Database?.GetCollection<RefreshToken>("refreshTokens");
        JwtService = jwtService;
    }

    public async Task<(string accessToken, string refreshToken)?> GetJwtTokens(User user)
    {
        if (user.Id == null) { return null; }
        
        var accessToken = JwtService.GenerateAccessToken(user.Id);
        var refreshTokenString = JwtService.GenerateRefreshToken();
        var refreshToken = await CreateRefreshToken(refreshTokenString, user.Id);

        if (refreshToken == null) { return null;}
        
        return (accessToken, refreshTokenString);
    }

    public async Task<string?> RefreshAccessToken(FilterDefinition<RefreshToken> filter)
    {
        var tokens = await GetAll(filter);
        var token = tokens?.FirstOrDefault();

        if (token == null)
        {
            return null;
        }
        var accessToken = JwtService.GenerateAccessToken(token.UserId);

        return accessToken;
    }
    
    public async Task<RefreshToken?> CreateRefreshToken(string token, string userId)
    {
        var refreshToken = new RefreshToken(
            token: token,
            userId: userId,
            expires: DateTime.Now.AddDays(7)
        );
        
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