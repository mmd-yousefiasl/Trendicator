using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace Application.Tools
{
    public static class IdentityTools
    {
        public static string GetUserIP(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.ToString();
        }

        public static int GetUserId(this ClaimsPrincipal claimsPrincipal)
        {
            if (claimsPrincipal == null)
                return default;

            if (claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier) == null)
                return default;

            string userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            if (string.IsNullOrEmpty(userId))
                return default;
            else
                return int.Parse(userId);
        }

        public static int GetUserId(this IPrincipal principal)
        {
            if (principal is null)
                return default;

            var user = (ClaimsPrincipal)principal;

            return user.GetUserId();
        }
    }
}
