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
}