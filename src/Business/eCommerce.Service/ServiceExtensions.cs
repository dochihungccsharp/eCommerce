using eCommerce.Service.AccessToken;
using eCommerce.Service.SendMail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Service;

public static class ServiceExtensions
{
    public static void AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        
        // Register services here
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<ISendMailService, SendMailService>();
        
    }
}