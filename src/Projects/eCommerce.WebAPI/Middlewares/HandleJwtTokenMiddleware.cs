using eCommerce.Service.AccessToken;

namespace eCommerce.WebAPI.Middlewares;

public class HandleJwtTokenMiddleware : IMiddleware
{
    private readonly IAccessTokenService _accessTokenService;

    public HandleJwtTokenMiddleware(IAccessTokenService accessTokenService)
    {
        _accessTokenService = accessTokenService;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                AttachUserToContext(context, token);
            }

            await next(context);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private void AttachUserToContext(HttpContext context, string token)
    {
        var auth = _accessTokenService.ParseJwtToken(token);
        context.Items["Token"] = token;
        context.Items["Auth"] = auth;
    }
}