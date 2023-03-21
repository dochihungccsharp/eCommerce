using eCommerce.Infrastructure.DatabaseRepository;
using eCommerce.Infrastructure.RoleRepository;
using eCommerce.Infrastructure.UserRepository;
using eCommerce.Infrastructure.UserRoleRepository;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Infrastructure;

public static class InfrastructureServiceExtensions
{
    public static void AddServicesInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<IDatabaseRepository, DatabaseRepository.DatabaseRepository>();
        services.AddScoped<IUserRepository, UserRepository.UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository.RoleRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository.UserRoleRepository>();
    }
}