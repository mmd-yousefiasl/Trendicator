using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace Web.Configuration
{
    public static class AuthenticationConfig
    {
        public static void ConfigAuthentication(this WebApplicationBuilder builder)
        {
            builder.Services.AddAuthentication(option =>
            {
                option.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                option.DefaultSignOutScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(option =>
            {
                option.LoginPath = "/login";
                option.LogoutPath = "/logout";
                option.ExpireTimeSpan = TimeSpan.FromDays(1);
                option.Cookie.SameSite = SameSiteMode.Lax;
                option.Cookie.MaxAge = TimeSpan.FromDays(1);
                option.SlidingExpiration = false;
                option.Cookie.IsEssential = true;
                option.Cookie.HttpOnly = true;
                option.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            })
            .AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
            {
                //builder.Configuration["Authentication:Google:ClientId"];
                options.ClientId = "asdasdasdasd";

                //builder.Configuration["Authentication:Google:ClientSecret"];
                options.ClientSecret = "asdasdasd"; 
            });
        }
    }
}
