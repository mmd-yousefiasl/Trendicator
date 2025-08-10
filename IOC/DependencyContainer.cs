using Application.Mediator;
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
            services.AddScoped<IUserValidationService, UserValidationService>();
            services.AddScoped<ISliderService, SliderService>();
            services.AddScoped<IBannerService, BannerService>();
            services.AddScoped<IFaqService, FaqService>();
            services.AddScoped<IOurFeaturesService, OurFeaturesService>();
            services.AddScoped<IBlogCategoryService, BlogCategoryService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogTagService, BlogTagService>();
            services.AddScoped<IBlogViewService, BlogViewService>();
            services.AddScoped<ISocialMediaService, SocialMediaService>();
            services.AddScoped<ITestimonialService, TestimonialService>();
            services.AddScoped<ISiteDataService, SiteDataService>();
            services.AddScoped<IEmailDataService, EmailDataService>();
            services.AddScoped<ISendEmailService, SendEmailService>();
            services.AddScoped<ISentLetterService, SentLetterService>();
            services.AddScoped<INewsLetterService, NewsLetterService>();
            services.AddScoped<IViewRenderService, ViewRenderService>();
            services.AddScoped<IContactUsFormService, ContactUsFormService>();

            #endregion

            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
        }
    }
}
