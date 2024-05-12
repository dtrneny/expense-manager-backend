using EmBackend.Models.Helpers;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace EmBackend.Services;

public class MongoDbService
{
    private readonly IOptions<DatabaseSettings> _settings;
    public IMongoDatabase? Database { get; }

    public MongoDbService(IOptions<DatabaseSettings> settings)
    {
        _settings = settings;
        
        var mongoClient = new MongoClient(settings.Value.ConnectionString);
        Database = mongoClient.GetDatabase(settings.Value.DatabaseName);
    }
}