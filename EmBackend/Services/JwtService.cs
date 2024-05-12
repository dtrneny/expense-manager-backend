using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using EmBackend.Entities;
using EmBackend.Models.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;

namespace EmBackend.Services;

public class JwtService
{
    private readonly IOptions<JwtSettings> _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings;
    }

    public string GenerateAccessToken(string userId)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        
        var tokenKey = _settings.Value.Key;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expiration = DateTime.Now.AddMinutes(_settings.Value.MinuteExpiration);
        
        List<Claim> claims = [
            new Claim("userId", userId)
        ];
        
        var token = new JwtSecurityToken(
            issuer:  _settings.Value.Issuer,
            audience:  _settings.Value.Audience,
            claims: claims,
            expires: expiration,
            signingCredentials: credentials
        );
        
        var accessToken = tokenHandler.WriteToken(token);
        
        if (accessToken == null)
        {
            // TODO: resolve with safer exit
            throw new Exception();
        }

        return accessToken;
    }

    public string GenerateRefreshToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    public string? GetUserIdFromClaimsPrincipal(ClaimsPrincipal? principal)
    {
        var userIdClaim = principal?.Claims.FirstOrDefault(claim => claim.Type == "userId");
        return userIdClaim?.Value;
    }
}