using Application.Services.Interfaces;
using Domain.Statics;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Implementaions
{
    public class VisitorService : IVisitorService
    {
        #region Fields
        private readonly IHttpContextAccessor _httpContextAccessor;
        #endregion

        #region Ctor
        public VisitorService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        #endregion

        public string GetOrCreateVisitorId()
        {
            var context = _httpContextAccessor.HttpContext;
            if (context == null) return Guid.NewGuid().ToString();

            if (context.Request.Cookies.TryGetValue(SiteTools.VisitorKeyName, out var existingId)
                && !string.IsNullOrWhiteSpace(existingId)
                && IsValidGuid(existingId))
            {
                return existingId;
            }

            var newVisitorId = Guid.NewGuid().ToString();

            context.Response.Cookies.Append(SiteTools.VisitorKeyName, newVisitorId, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Expires = DateTimeOffset.UtcNow.AddDays(SiteTools.VisitorExpirationDays),
                IsEssential = true
            });

            return newVisitorId;
        }

        private static bool IsValidGuid(string value)
        {
            return Guid.TryParse(value, out _);
        }
    }
}
