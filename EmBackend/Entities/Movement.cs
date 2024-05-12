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
    public required int Amount { get; set; }
    
    [BsonElement("label")]
    [JsonPropertyName("label")]
    public required string Label { get; set; }
}