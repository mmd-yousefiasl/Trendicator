using Application.Services.Implementaions;
using Application.Services.Interfaces;
using Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IOC
{
    public class DependencyContainer
    {
        public static void RegisterDependencies(IServiceCollection services, string connectionString)
        {
            #region DataBase

            services.AddDbContext<TrendicatorDbContext>(options =>
            {
                options.UseSqlServer(connectionString);
            });

            #endregion

            #region Services

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();

            #endregion

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        }
    }
}
