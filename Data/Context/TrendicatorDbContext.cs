using Domain.Models.Blogs;
using Domain.Models.ContactUs;
using Domain.Models.Coupons;
using Domain.Models.Faqs;
using Domain.Models.Identity;
using Domain.Models.NewsLetters;
using Domain.Models.Products;
using Domain.Models.Shopping;
using Domain.Models.SiteInfo;
using Domain.Models.SiteNotifications;
using Domain.Models.Tickets;
using Domain.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class TrendicatorDbContext : DbContext
    {
        public TrendicatorDbContext(DbContextOptions<TrendicatorDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TrendicatorDbContext).Assembly);
            modelBuilder.Seed();
        }

        public DbSet<Blog> Blogs => Set<Blog>();
        public DbSet<BlogCategory> BlogCategories => Set<BlogCategory>();
        public DbSet<BlogView> BlogViews => Set<BlogView>();
        public DbSet<BlogComment> BlogComments => Set<BlogComment>();

        public DbSet<ContactUsForm> ContactUsForms => Set<ContactUsForm>();

        public DbSet<Coupon> Coupons => Set<Coupon>();

        public DbSet<Faq> Faqs => Set<Faq>();

        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<RolePermissionMapping> RolePermissionMappings => Set<RolePermissionMapping>();

        public DbSet<NewsLetter> NewsLetters => Set<NewsLetter>();
        public DbSet<SentLetter> SentLetters => Set<SentLetter>();

        public DbSet<ProductCategory> ProductCategories => Set<ProductCategory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductFaq> ProductFaqs => Set<ProductFaq>();
        public DbSet<ProductReview> ProductReviews => Set<ProductReview>();
        public DbSet<ProductView> ProductViews => Set<ProductView>();
        public DbSet<ProductFeature> ProductFeatures => Set<ProductFeature>();
        public DbSet<ProductDetails> ProductDetails => Set<ProductDetails>();

        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<UserOrderStats> UserOrderStats => Set<UserOrderStats>();

        public DbSet<Banner> Banners => Set<Banner>();
        public DbSet<EmailData> EmailData => Set<EmailData>();
        public DbSet<Hero> Heroes => Set<Hero>();
        public DbSet<Media> Media => Set<Media>();
        public DbSet<OurFeature> OurFeatures => Set<OurFeature>();
        public DbSet<PaymentData> PaymentData => Set<PaymentData>();
        public DbSet<SiteData> SiteData => Set<SiteData>();
        public DbSet<SiteMap> SiteMap => Set<SiteMap>();
        public DbSet<SmsPanel> SmsPanels => Set<SmsPanel>();
        public DbSet<SocialMedia> SocialMedia => Set<SocialMedia>();
        public DbSet<Testimonial> Testimonials => Set<Testimonial>();

        public DbSet<SiteNotification> SiteNotifications => Set<SiteNotification>();

        public DbSet<Ticket> Tickets => Set<Ticket>();
        public DbSet<TicketMessage> TicketMessages => Set<TicketMessage>();

        public DbSet<BalanceTransaction> BalanceTransactions => Set<BalanceTransaction>();
        public DbSet<User> Users => Set<User>();
        public DbSet<UserRoleMapping> UserRoleMappings => Set<UserRoleMapping>();
        public DbSet<UserValidation> UserValidation => Set<UserValidation>();
    }
}
