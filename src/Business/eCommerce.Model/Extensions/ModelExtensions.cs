using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eCommerce.Model.Extensions;

public static class ModelExtensions
{
    public static void AddFluentValidator(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers()
            .AddFluentValidation(options =>
            {
                options.DisableDataAnnotationsValidation = true;
                
                options.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());

                ValidatorOptions.Global.DefaultClassLevelCascadeMode = CascadeMode.Stop;
                
                ValidatorOptions.Global.DefaultRuleLevelCascadeMode = CascadeMode.Stop;
            });
    }
}