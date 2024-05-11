namespace EmBackend.Configurations;

public static class RoutesConfiguration
{
    public static void ConfigureRoutes(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
}