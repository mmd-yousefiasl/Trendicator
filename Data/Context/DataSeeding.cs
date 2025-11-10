using Domain.Models.Identity;
using Domain.Models.SiteInfo;
using Domain.Models.Users;
using Domain.Statics;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public static class DataSeeding
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            #region Permission

            modelBuilder.Entity<Permission>().HasData(PermissionsList.Permissions);

            #endregion

            #region Identity 

            var adminRole = new Role()
            {
                Id = 1,
                Title = "Admin",
                UniqueName = "Admin"
            };
            modelBuilder.Entity<Role>().HasData(adminRole);

            var rolePermissionMapping = new RolePermissionMapping()
            {
                RoleId = 1,
                PermissionId = 1
            };
            modelBuilder.Entity<RolePermissionMapping>().HasData(rolePermissionMapping);

            #endregion

            #region User

            User AdminUser = new User()
            {
                Id = 1,
                FirstName = "Mohammad",
                LastName = "Yousefi",
                Email = "mmdyousefi95@gmail.com",
                IsUserBanned = false,
                PhoneNumber = "09962144266",
            };

            modelBuilder.Entity<User>().HasData(AdminUser);

            #endregion

            #region UserRoleMapping

            UserRoleMapping adminUserRoleMapping = new UserRoleMapping
            {
                UserId = 1,
                RoleId = 1
            };

            modelBuilder.Entity<UserRoleMapping>().HasData(adminUserRoleMapping);

            #endregion

            #region SiteData 

            SiteData siteData = new SiteData()
            {
                Id = 1,
                SiteName = "ترندیکاتور",
            };
            modelBuilder.Entity<SiteData>().HasData(siteData);

            PaymentData paymentData = new PaymentData()
            {
                Id = 1,
                IsDeleted = false,
                Tax = 9,
                PaymentGatewayPin = "zibal"
            };
            modelBuilder.Entity<PaymentData>().HasData(paymentData);

            #endregion

            #region EmailData
            EmailData emailData = new EmailData()
            {
                Id = 1,
            };

            modelBuilder.Entity<EmailData>().HasData(emailData);
            #endregion

            #region SmsData
            SmsData smsData = new SmsData()
            {
                Id = 1,
                IsDeleted = false,
                SmsPanelAPIKey = "70C2f6O6S8XtW52ue9G83hRL3Fsz2YqIZ02Gm2eUMZjXkPSP",
                SmsPatternIdForLoginCode = 409943
            };

            modelBuilder.Entity<SmsData>().HasData(smsData);
            #endregion
        }
    }
}
