using Asp.Versioning;

namespace EmBackend.Configurations;

public static class RoutesConfiguration
{
    public static void ConfigureRoutes(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.DefaultApiVersion = new ApiVersion(1, 0);
            options.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        
        services.Configure<RouteOptions>(options => options.LowercaseUrls = true);
    }
    
    public static void ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options => {
            options.AddPolicy(name: "cors", policy =>
            {
                policy
                    .WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });
    }
}