using EmBackend.Models.Helpers;
using EmBackend.Services;
using EmBackend.Services.HashService;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace EmBackend.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager builder)
    {
        services.Configure<DatabaseSettings>(builder.GetSection("MongoDatabase"));
        services.AddSingleton<MongoDbService>();
        
        var pack = new ConventionPack
        {
            new EnumRepresentationConvention(BsonType.String)
        };

        ConventionRegistry.Register("EnumStringConvention", pack, t => true);
    }
    
    public static void ConfigureHashService(this IServiceCollection services)
    {
        services.AddScoped<IHashService, HashService>();
    }
    
    public static void ConfigureJwtService(this IServiceCollection services)
    {
        services.AddScoped<JwtService>();
    }
    
    public static void ConfigureBaseServices(this IServiceCollection services)
    {
        services.AddScoped<FileService>();
    }
}