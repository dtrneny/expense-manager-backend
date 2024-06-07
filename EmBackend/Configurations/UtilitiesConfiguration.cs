using EmBackend.Utilities;
using Microsoft.OpenApi.Models;

namespace EmBackend.Configurations;

public static class UtilitiesConfiguration
{
    public static void ConfigureUtilities(this IServiceCollection services)
    {
        services.AddScoped<Validation>();
        services.AddScoped<EntityMapper>();
        services.AddScoped<CompressionUtility>();
    }
    
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}