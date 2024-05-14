using System.Text.Json.Serialization;
using EmBackend.Entities.Helpers;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace EmBackend.Entities;

public class Category
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    
    [BsonElement("name")]
    [JsonPropertyName("name")]
    public required string Name { get; set; }

    [BsonElement("ownership")]
    [JsonPropertyName("ownership")]
    public required CategoryOwnership Ownership { get; set; } = CategoryOwnership.Default;
    
    [BsonElement("owner_id")]
    [JsonPropertyName("owner_id")]
    public string? OwnerId { get; set; } = null;
}