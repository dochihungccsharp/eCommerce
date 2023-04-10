using System.Net;
using System.Web.Http.Results;
using eCommerce.Model.Abstractions.Responses;
using eCommerce.Model.Users;
using eCommerce.Service.Cache.RoleCache;
using eCommerce.Shared.Consts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eCommerce.WebAPI.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _roles;
    
    public AuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }
    
    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {

        var _roleCacheService = (RoleCacheService)context.HttpContext.RequestServices.GetService(typeof(IRoleCacheService));
        var _authModel = (UserContextModel)context.HttpContext.RequestServices.GetService(typeof(UserContextModel));
            
        if (_authModel == null)
        {
            context.Result = new JsonResult(new BaseResponseModel(HttpStatusCode.Unauthorized, "Unauthorized"));
            return;
        }
            
        if (_roles == null || _roles.Length < 1) return;
            
        var userId = Guid.Parse(_authModel.Id);
        var userRoles = await _roleCacheService.GetUserRolesAsync(userId);

        if (userRoles == null || userRoles.Count < 1)
        {
            context.Result = new JsonResult(new BaseResponseModel(HttpStatusCode.Unauthorized, "Forbidden"));
            return;
        }
           
        if(userRoles.Contains(Roles.Admin)) return;

         foreach (var role in _roles)
        {
            if (!userRoles.Contains(role))
            {
                context.Result = new JsonResult(new BaseResponseModel(HttpStatusCode.Unauthorized, "Forbidden"));
                return;
            }
        }
    }
        
}