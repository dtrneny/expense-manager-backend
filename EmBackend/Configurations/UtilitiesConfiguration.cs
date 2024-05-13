using EmBackend.Utilities;

namespace EmBackend.Configurations;

public static class UtilitiesConfiguration
{
    public static void ConfigureUtilities(this IServiceCollection services)
    {
        services.AddScoped<Validation>();
    }
}