using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmBackend.Entities;

public class RefreshToken
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [Required]
    [JsonPropertyName("token")]
    public string Token { get; set; }
    
    [Required]
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
    
    [Required]
    [JsonPropertyName("expires")]
    public DateTime Expires { get; set; }

    public RefreshToken(string token, string userId, DateTime expires)
    {
        Id = null;
        Token = token;
        UserId = userId;
        Expires = expires;
    }
}