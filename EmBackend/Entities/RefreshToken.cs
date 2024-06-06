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
    
    [BsonElement("token")]
    [JsonPropertyName("token")]
    public required string Token { get; set; }
    
    [BsonElement("user_id")]
    [JsonPropertyName("userId")]
    public required string UserId { get; set; }
    
    [BsonElement("expires")]
    [JsonPropertyName("expires")]
    public DateTime Expires { get; set; }

    [BsonElement("access_token")]
    [JsonPropertyName("access_token")]
    public required string AccessToken { get; set; }
}