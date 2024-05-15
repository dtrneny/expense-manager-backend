using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmBackend.Entities;

public class User
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("firstname")]
    [JsonPropertyName("firstname")]
    public required string Firstname { get; set; }
    
    [BsonElement("lastname")]
    [JsonPropertyName("lastname")]
    public required string Lastname { get; set; }
    
    [BsonElement("email")]
    [JsonPropertyName("email")]
    public required string Email { get; set; }
    
    [BsonElement("password")]
    [JsonPropertyName("password")]
    public required string Password { get; set; }

    [BsonElement("balance")]
    [JsonPropertyName("balance")]
    public double Balance { get; set; } = 0;
}