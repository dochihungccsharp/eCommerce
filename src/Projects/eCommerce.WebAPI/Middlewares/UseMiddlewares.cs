namespace eCommerce.WebAPI.Middlewares;

public static class UseMiddlewares
{
    public static void UseExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<HandleExceptionMiddleware>();
    }
        
    public static void UseJwtTokenMiddleware(this WebApplication app)
    {
        app.UseMiddleware<HandleJwtTokenMiddleware>();
    }
}