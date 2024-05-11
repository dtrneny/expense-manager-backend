using EmBackend.Helpers;
using EmBackend.Services;

namespace EmBackend.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this IServiceCollection services, ConfigurationManager builder)
    {
        services.Configure<DatabaseSettings>(builder.GetSection("MongoDatabase"));
        services.AddSingleton<MongoDbService>();
    }
}