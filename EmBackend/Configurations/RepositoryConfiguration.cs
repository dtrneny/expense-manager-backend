using EmBackend.Entities;
using EmBackend.Repositories;
using EmBackend.Repositories.Auth;

namespace EmBackend.Configurations;

public static class RepositoryConfiguration
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, UserRepository>();
        services.AddScoped<AuthRepository>();
    }
}