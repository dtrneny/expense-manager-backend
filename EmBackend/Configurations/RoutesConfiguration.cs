namespace EmBackend.Configurations;

public static class RoutesConfiguration
{
    public static void ConfigureRoutes(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
    
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => {
            options.AddPolicy(name: "test", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}