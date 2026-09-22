using Application.Mediator;
using Application.Services.Implementaions;
using Application.Services.Interfaces;
using Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Web.BackgroundServices;

namespace Web
{
    public class DependencyContainer
    {
        public static void RegisterDependencies(IServiceCollection services, string connectionString)
        {
            #region Config DataBase
            services.AddDbContextPool<TrendicatorDbContext>(options =>
            {
                options.UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(5),
                            errorCodesToAdd: null);
                    }
                );

                options.ConfigureWarnings(warnings =>
                    warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
            });
            #endregion

            #region Singleton Services
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<ISmsQueue, SmsQueue>();
            #endregion

            #region HttpClient Services
            services.AddHttpClient<ITelegramService, TelegramService>();
            #endregion

            #region Scoped Services
            services.AddScoped<IMediatorHandler, MediatorHandler>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IPermissionService, PermissionService>();
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
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductOptionService, ProductOptionService>();
            services.AddScoped<IProductViewService, ProductViewService>();
            services.AddScoped<IProductWishListService, ProductWishListService>();
            services.AddScoped<IProductCompareService, ProductCompareService>();
            services.AddScoped<IProductReviewService, ProductReviewService>();
            services.AddScoped<ICouponService, CouponService>();
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IPaymentDataService, PaymentDataService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IBalanceTransactionService, BalanceTransactionService>();
            services.AddScoped<ITicketService, TicketService>();
            services.AddScoped<ISiteNotificationService, SiteNotificationService>();
            services.AddScoped<ISmsPanelService, SmsDataService>();
            services.AddScoped<ISendSmsService, SendSmsService>();
            services.AddScoped<IHeroService, HeroService>();
            services.AddScoped<ISiteMapService, SiteMapService>();
            services.AddScoped<IMediaService, MediaService>();
            services.AddScoped<IProductFaqService, ProductFaqService>();
            #endregion

            #region Background Services
            services.AddHostedService<SmsConsumerWorker>();
            #endregion
        }
    }
}
