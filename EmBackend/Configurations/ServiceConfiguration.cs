using EmBackend.Entities;
using EmBackend.Repositories;
using EmBackend.Repositories.Users;

namespace EmBackend.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureRepositories(this IServiceCollection services)
    {
        services.AddScoped<IRepository<User>, UserRepository>();
    }
}