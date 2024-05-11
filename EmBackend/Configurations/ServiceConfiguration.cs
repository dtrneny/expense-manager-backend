using EmBackend.Helpers;
using EmBackend.Services;
using EmBackend.Services.HashService;

namespace EmBackend.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager builder)
    {
        services.Configure<DatabaseSettings>(builder.GetSection("MongoDatabase"));
        services.AddSingleton<MongoDbService>();
    }
    
    public static void ConfigureHashService(this IServiceCollection services)
    {
        services.AddScoped<IHashService, HashService>();
    }
}