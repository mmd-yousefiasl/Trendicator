using Application.Services.Interfaces;
using Application.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Web.Attributes
{
    public class Permission : AuthorizeAttribute, IAsyncAuthorizationFilter
    {
        private readonly string _permissionName;

        public Permission(string permissionName)
        {
            _permissionName = permissionName;
        }
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var _roleService = context.HttpContext.RequestServices.GetRequiredService<IRoleService>();

            if (context.HttpContext.User?.Identity?.IsAuthenticated ?? false)
            {
                var userId = context.HttpContext.User.GetUserId();
                var userHasPermissionResult = await _roleService.UserCanAccessThisPermissions(userId, _permissionName);

                if (userHasPermissionResult.IsFailure || userHasPermissionResult.Value == false)
                {
                    context.Result = new RedirectResult("/403");
                }
            }
            else
            {
                context.Result = new RedirectResult("/login");
            }
        }
    }
}
