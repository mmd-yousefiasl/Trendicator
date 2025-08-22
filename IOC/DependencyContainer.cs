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
            services.AddScoped<IProductCategoryService, ProductCategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductGalleryService, ProductGalleryService>();
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

            #endregion

            services.AddSingleton<ICacheManager, CacheManager>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IMediatorHandler, MediatorHandler>();
        }
    }
}
