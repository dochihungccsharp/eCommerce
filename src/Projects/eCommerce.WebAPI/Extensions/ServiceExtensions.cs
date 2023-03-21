using eCommerce.Infrastructure;
using eCommerce.Model;
using eCommerce.Model.Users;
using eCommerce.Service;
using eCommerce.Shared.Configurations;

namespace eCommerce.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddConfigurationSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions();
        var databaseSetting = configuration.GetSection("DatabaseSetting");
        services.Configure<DatabaseSetting>(databaseSetting);
        
        var mailSetting = configuration.GetSection("MailSetting");
        services.Configure<MailSetting>(mailSetting);

        var jwtSetting = configuration.GetSection("JwtSetting");
        services.Configure<JwtSetting>(jwtSetting);

        return services;
    }
    
    public static IServiceCollection AddUserContextModelService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddScoped(serviceProvider =>
        {
            var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();

            return (UserContextModel)httpContextAccessor.HttpContext.Items["Auth"] ?? default!;
        });

        return services;
    }

    public static IServiceCollection AddService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddFluentValidator(configuration);
        return services;
    }
    
    public static IServiceCollection AddModelService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddServices(configuration);
        return services;
    }
    
    public static IServiceCollection AddInfrastructureService(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddServicesInfrastructure();
        return services;
    }
}