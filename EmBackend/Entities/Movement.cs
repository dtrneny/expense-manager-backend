using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmBackend.Entities;

public class Movement
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("user_id")]
    [JsonPropertyName("user_id")]
    public required string UserId { get; set; }
    
    [BsonElement("amount")]
    [JsonPropertyName("amount")]
    public required double Amount { get; set; }
    
    [BsonElement("label")]
    [JsonPropertyName("label")]
    public required string Label { get; set; }
    
    [BsonElement("timestamp")]
    [JsonPropertyName("timestamp")]
    public required DateTime Timestamp { get; set; }

    [BsonElement("category_ids")]
    [JsonPropertyName("category_ids")]
    public List<string> CategoryIds { get; set; } = [];
}