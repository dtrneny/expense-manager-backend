using EmBackend.Entities;
using EmBackend.Models.Helpers;
using EmBackend.Repositories;
using EmBackend.Repositories.Interfaces;
using EmBackend.Services;
using EmBackend.Services.Interfaces;
using EmBackend.Utilities;
using Microsoft.OpenApi.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Conventions;

namespace EmBackend.Configurations;

public static class BaseApiConfiguration
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
    
    public static void ConfigureUtilities(this IServiceCollection services)
    {
        services.AddScoped<Validation>();
        services.AddScoped<EntityMapper>();
    }
    
    public static void ConfigureServices(this IServiceCollection services)
    {
        services.AddScoped<JwtService>();
        services.AddScoped<FileService>();
        services.AddScoped<IHashService, HashService>();
    }
    
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<AuthRepository>();
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<IRepository<Movement>, MovementRepository>();
        services.AddScoped<IRepository<Category>, CategoryRepository>();
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
    
    public static void ConfigureSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
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