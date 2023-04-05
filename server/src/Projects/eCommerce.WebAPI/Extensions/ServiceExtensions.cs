using System.Text;
using eCommerce.Infrastructure;
using eCommerce.Model;
using eCommerce.Model.Users;
using eCommerce.Service;
using eCommerce.Shared.Configurations;
using eCommerce.Shared.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace eCommerce.WebAPI.Extensions;

public static class ServiceExtensions
{
    public static IServiceCollection AddHangfire(this IServiceCollection services, WebApplicationBuilder builder)
    {
        var databaseSetting = builder.Configuration.GetOptions<DatabaseSetting>();
        
        //Register Hangfire services
        services.AddHangfire(configuration => configuration
            .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
            .UseRecommendedSerializerSettings()
            .UseSqlServerStorage(databaseSetting.Default)
        );

        // Register the Hangfire dashboard authorization filter
        //services.AddHangfireDashboardAuthorization();
        
        return services;
    }
    public static IServiceCollection AddControllerService(this IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(options =>
        {
            options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            options.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };
        });
        return services;
    }

    public static IServiceCollection AddAuthenticationService(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSetting = configuration.GetOptions<JwtSetting>();
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSetting.Key)),
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = jwtSetting.Issuer,
                    ValidAudience = jwtSetting.Audience
                };
            });

        return services;
    }
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

    public static IServiceCollection AddModelService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddFluentValidator(configuration);
        return services;
    }
    
    public static IServiceCollection AddService(this IServiceCollection services,
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

    public static IServiceCollection AddSwaggerService(this IServiceCollection services)
    {
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
    
            // Configure Swagger to use the JWT bearer authentication scheme
            var securityScheme = new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            };
            c.AddSecurityDefinition("Bearer", securityScheme);
    
            // Make Swagger require a JWT token to access the endpoints
            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    securityScheme,
                    new string[] {}
                }
            });
        });
        return services;
    }
}