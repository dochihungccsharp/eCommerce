using eCommerce.Service.AccessToken;
using eCommerce.Service.Brands;
using eCommerce.Service.Cache.RoleCache;
using eCommerce.Service.Categories;
using eCommerce.Service.Products;
using eCommerce.Service.Provinces;
using eCommerce.Service.PurchaseOrders;
using eCommerce.Service.Roles;
using eCommerce.Service.SendMail;
using eCommerce.Service.Suppliers;
using eCommerce.Service.Uploads;
using eCommerce.Service.Users;
using Microsoft.Extensions.Caching.Memory;
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
        services.AddScoped<IMemoryCache, MemoryCache>();
        services.AddScoped<IAccessTokenService, AccessTokenService>();
        services.AddScoped<ISendMailService, SendMailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IRoleCacheService, RoleCacheService>();
        
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IPurchaseOrderService, PurchaseOrderService>();
        services.AddScoped<IUploadService, UploadService>();

       

    }
}