using Domain.Models.Identity;
using System.Net.Http.Headers;

namespace Domain.Statics
{
    public class PermissionsList
    {
        private static readonly List<Permission> _permissions = new List<Permission>()
        {
            new Permission
            {
                Id = 1,
                ParentId = null,
                Title = "Admin Panel",
                UniqueName = "AdminPanel"
            },

            #region User
            new Permission
            {
                Id = 2,
                ParentId = 1,
                Title = "مدیریت کاربران",
                UniqueName = "ManageUsers"
            },
            new Permission
            {
                Id = 3,
                ParentId = 2,
                Title = "لیست کاربران",
                UniqueName = "UsersList"
            },
            new Permission
            {
                Id = 4,
                ParentId = 2,
                Title = "افزودن کاربر جدید",
                UniqueName = "CreateNewUser"
            },
            new Permission
            {
                Id = 5,
                ParentId = 2,
                Title = "ویرایش کاربر",
                UniqueName = "UpdateUser"
            },
            new Permission
            {
                Id = 6,
                ParentId = 2,
                Title = "حذف کاربر",
                UniqueName = "DeleteUser"
            },
            #endregion

            #region Role
            new Permission
            {
                Id = 7,
                ParentId = 1,
                Title = "مدیریت نقش ها",
                UniqueName = "ManageRoles"
            },
            new Permission
            {
                Id = 8,
                ParentId = 7,
                Title = "لیست نقش ها",
                UniqueName = "RolesList"
            },
            new Permission
            {
                Id = 9,
                ParentId = 7,
                Title = "افزودن نقش جدید",
                UniqueName = "CreateNewRole"
            },
            new Permission
            {
                Id = 10,
                ParentId = 7,
                Title = "ویرایش نقش",
                UniqueName = "UpdateRole"
            },
            new Permission
            {
                Id = 11,
                ParentId = 7,
                Title = "حذف نقش",
                UniqueName = "DeleteRole"
            },
            #endregion

            #region Slider
            new Permission
            {
                Id = 12,
                ParentId = 1,
                Title = "مدیریت لغزنده ها",
                UniqueName = "ManageSlider"
            },
            new Permission
            {
                Id = 13,
                ParentId = 12,
                Title = "لیست لغزنده ها",
                UniqueName = "SlidersList"
            },
            new Permission
            {
                Id = 14,
                ParentId = 12,
                Title = "افزودن لغزنده جدید",
                UniqueName = "CreateNewSlider"
            },
            new Permission
            {
                Id = 15,
                ParentId = 12,
                Title = "ویرایش لغزنده",
                UniqueName = "UpdateSlider"
            },
            new Permission
            {
                Id = 16,
                ParentId = 12,
                Title = "حذف لغزنده",
                UniqueName = "DeleteSlider"
            },
            #endregion

            #region Banner
            new Permission
            {
                Id = 17,
                ParentId = 1,
                Title = "مدیریت بنر ها",
                UniqueName = "ManageBanners"
            },
            new Permission
            {
                Id = 18,
                ParentId = 17,
                Title = "لیست بنر ها",
                UniqueName = "BannersList"
            },
            new Permission
            {
                Id = 19,
                ParentId = 17,
                Title = "افزودن بنر جدید",
                UniqueName = "CreateNewBanner"
            },
            new Permission
            {
                Id = 20,
                ParentId = 17,
                Title = "ویرایش بنر",
                UniqueName = "UpdateBanner"
            },
            new Permission
            {
                Id = 21,
                ParentId = 17,
                Title = "حذف بنر",
                UniqueName = "DeleteBanner"
            },
            #endregion

            #region Faq
            new Permission
            {
                Id = 22,
                ParentId = 1,
                Title = "مدیریت سوالات متداول",
                UniqueName = "ManageFaqs"
            },
            new Permission
            {
                Id = 23,
                ParentId = 22,
                Title = "لیست سوالات متداول",
                UniqueName = "FaqsList"
            },
            new Permission
            {
                Id = 24,
                ParentId = 22,
                Title = "افزودن سوال متداول جدید",
                UniqueName = "CreateNewFaq"
            },
            new Permission
            {
                Id = 25,
                ParentId = 22,
                Title = "ویرایش سوال متداول",
                UniqueName = "UpdateFaq"
            },
            new Permission
            {
                Id = 26,
                ParentId = 22,
                Title = "حذف سوال متداول",
                UniqueName = "DeleteFaq"
            },
            #endregion

            #region Our Feature
            new Permission
            {
                Id = 27,
                ParentId = 1,
                Title = "مدیریت ویژگی های ما",
                UniqueName = "ManageOurFeatures"
            },
            new Permission
            {
                Id = 28,
                ParentId = 27,
                Title = "لیست ویژگی های ما",
                UniqueName = "OurFeaturesList"
            },
            new Permission
            {
                Id = 29,
                ParentId = 27,
                Title = "افزودن ویژگی جدید",
                UniqueName = "CreateNewOurFeature"
            },
            new Permission
            {
                Id = 30,
                ParentId = 27,
                Title = "ویرایش ویژگی ما",
                UniqueName = "UpdateOurFeature"
            },
            new Permission
            {
                Id = 31,
                ParentId = 27,
                Title = "حذف ویژگی ما",
                UniqueName = "DeleteOurFeature"
            },
            #endregion

            #region BlogCategory
            new Permission
            {
                Id = 32,
                ParentId = 1,
                Title = "مدیریت دستی بندی مقالات",
                UniqueName = "ManageBlogCategories"
            },
            new Permission
            {
                Id = 33,
                ParentId = 32,
                Title = "لیست دسته بندی ها",
                UniqueName = "BlogCategoriesList"
            },
            new Permission
            {
                Id = 34,
                ParentId = 32,
                Title = "افزودن دسته بندی جدید",
                UniqueName = "CreateNewBlogCategory"
            },
            new Permission
            {
                Id = 35,
                ParentId = 32,
                Title = "ویرایش دسته بندی",
                UniqueName = "UpdateBlogCategory"
            },
            new Permission
            {
                Id = 36,
                ParentId = 32,
                Title = "حذف دسته بندی",
                UniqueName = "DeleteBlogCategory"
            },
            #endregion

            #region BlogTag
            new Permission
            {
                Id = 37,
                ParentId = 1,
                Title = "مدیریت برچسب مقالات",
                UniqueName = "ManageBlogTags"
            },
            new Permission
            {
                Id = 38,
                ParentId = 37,
                Title = "لیست برچسب ها",
                UniqueName = "BlogTagsList"
            },
            new Permission
            {
                Id = 39,
                ParentId = 37,
                Title = "افزودن برچسب جدید",
                UniqueName = "CraeteNewBlogTag"
            },
            new Permission
            {
                Id = 40,
                ParentId = 37,
                Title = "ویرایش برچسب",
                UniqueName = "UpdateBlogTag"
            },
            new Permission
            {
                Id = 41,
                ParentId = 37,
                Title = "حذف برچسب",
                UniqueName = "DeleteBlogTag"
            },
            #endregion

            #region Blog
            new Permission
            {
                Id = 42,
                ParentId = 1,
                Title = "مدیریت مقالات",
                UniqueName = "ManageBlogs"
            },
            new Permission
            {
                Id = 43,
                ParentId = 42,
                Title = "لیست مقالات",
                UniqueName = "BlogsList"
            },
            new Permission
            {
                Id = 44,
                ParentId =42,
                Title = "افزودن مقاله جدید",
                UniqueName = "CreateNewBlog"
            },
            new Permission
            {
                Id = 45,
                ParentId = 42,
                Title = "ویرایش مقاله",
                UniqueName = "UpdateBlog"
            },
            new Permission
            {
                Id = 46,
                ParentId = 42,
                Title = "حذف مقاله",
                UniqueName = "DeleteBlog"
            },
            #endregion

            #region SocialMedia
            new Permission
            {
                Id = 47,
                ParentId = 1,
                Title = "مدیریت شبکه های اجتماعی",
                UniqueName = "ManageSocialMedia"
            },
            new Permission
            {
                Id = 48,
                ParentId = 47,
                Title = "لیست شبکه های اجتماعی",
                UniqueName = "SocialMediaList"
            },
            new Permission
            {
                Id = 49,
                ParentId = 47,
                Title = "افزودن شبکه اجتماعی جدید",
                UniqueName = "CreateNewSocialMedia"
            },
            new Permission
            {
                Id = 50,
                ParentId = 47,
                Title = "ویرایش شبکه اجتماعی",
                UniqueName = "UpdateSocialMedia"
            },
            new Permission
            {
                Id = 51,
                ParentId = 47,
                Title = "حذف شبکه اجتماعی",
                UniqueName = "DeleteSocialMedia"
            },
            #endregion

            #region Testimonial
            new Permission
            {
                Id = 52,
                ParentId = 1,
                Title = "مدیریت گواهی ها",
                UniqueName = "ManageTestimonials"
            },
            new Permission
            {
                Id = 53,
                ParentId = 52,
                Title = "لیست گواهی ها",
                UniqueName = "TestimonialsList"
            },
            new Permission
            {
                Id = 54,
                ParentId = 52,
                Title = "افزودن گواهی جدید",
                UniqueName = "CreateNewTestimonial"
            },
            new Permission
            {
                Id = 55,
                ParentId = 52,
                Title = "ویرایش گواهی",
                UniqueName = "UpdateTestimonial"
            },
            new Permission
            {
                Id = 56,
                ParentId = 52,
                Title = "حذف گواهی",
                UniqueName = "DeleteTestimonial"
            },
            #endregion

            #region SiteData
            new Permission
            {
                Id = 57,
                ParentId = 1,
                Title = "مدیریت اطلاعات سایت",
                UniqueName = "ManageSiteData"
            },
            new Permission
            {
                Id = 58,
                ParentId = 57,
                Title = "ویرایش اطلاعات سایت",
                UniqueName = "UpdateSiteData"
            },
            #endregion

            #region EmailData
            new Permission
            {
                Id = 59,
                ParentId = 1,
                Title = "مدیریت اطلاعات ایمیل",
                UniqueName = "ManageEmailData"
            },
            new Permission
            {
                Id = 60,
                ParentId = 59,
                Title = "ویرایش اطلاعات ایمیل",
                UniqueName = "UpdateEmailData"
            },
            #endregion

            #region NewsLetters
            new Permission
            {
                Id = 61,
                ParentId = 1,
                Title = "مدیریت ایمیل های خبرنامه",
                UniqueName = "ManageNewsLetters"
            },
            new Permission
            {
                Id = 62,
                ParentId = 61,
                Title = "لیست ایمیل های خبرنامه",
                UniqueName = "NewsLettersList"
            },
            new Permission
            {
                Id = 63,
                ParentId = 61,
                Title = "حذف ایمیل خبرنامه",
                UniqueName = "DeleteNewsLetters"
            },
            #endregion

            #region SentLetter
            new Permission
            {
                Id = 64,
                ParentId = 1,
                Title = "مدیریت ایمیل های ارسال شده",
                UniqueName = "ManageSentLetters"
            },
            new Permission
            {
                Id = 65,
                ParentId = 64,
                Title = "لیست ایمیل های ارسال شده",
                UniqueName = "SentLettersList"
            },
            new Permission
            {
                Id = 66,
                ParentId = 64,
                Title = "حذف ایمیل ارسال شده",
                UniqueName = "DeleteSentLetter"
            },
            new Permission
            {
                Id = 67,
                ParentId = 64,
                Title = "ارسال ایمیل جدید",
                UniqueName = "SendLetter"
            },
            #endregion

            #region ContactUs
            new Permission
            {
                Id = 68,
                ParentId = 1,
                Title = "مدیریت فرم های تماس باما",
                UniqueName = "ManageContactUs"
            },
            new Permission
            {
                Id = 69,
                ParentId = 68,
                Title = "لیست فرم های تماس باما",
                UniqueName = "ContactUsList"
            },
            new Permission
            {
                Id = 70,
                ParentId = 68,
                Title = "حذف فرم تماس باما",
                UniqueName = "DeleteContactUsForm"
            },
            #endregion

            #region ProductCategory
            new Permission
            {
                Id = 71,
                ParentId = 1,
                Title = "مدیریت دسته بندی های محصولات",
                UniqueName = "ManageProductCategories"
            },
            new Permission
            {
                Id = 72,
                ParentId = 71,
                Title = "لیست دسته بندی های محصولات",
                UniqueName = "ProductCategoriesList"
            },
            new Permission
            {
                Id = 73,
                ParentId = 71,
                Title = "افزودن دسته بندی جدید",
                UniqueName = "CreateNewProductCategory"
            },
            new Permission
            {
                Id = 74,
                ParentId = 71,
                Title = "ویرایش دسته بندی",
                UniqueName = "UpdateProductCategory"
            },
            new Permission
            {
                Id = 75,
                ParentId = 71,
                Title = "حذف دسته بندی",
                UniqueName = "DeleteProductCategory"
            },
            #endregion

            #region Product
            new Permission
            {
                Id = 76,
                ParentId = 1,
                Title = "مدیریت محصولات",
                UniqueName = "ManageProducts"
            },
            new Permission
            {
                Id = 77,
                ParentId = 76,
                Title = "لیست محصولات",
                UniqueName = "ProductsList"
            },
            new Permission
            {
                Id = 78,
                ParentId = 76,
                Title = "افزودن محصول جدید",
                UniqueName = "CreateNewProduct"
            },
            new Permission
            {
                Id = 79,
                ParentId = 76,
                Title = "ویرایش محصول",
                UniqueName = "UpdateProduct"
            },
            new Permission
            {
                Id = 80,
                ParentId = 76,
                Title = "حذف محصول",
                UniqueName = "DeleteProduct"
            },
            #endregion

            #region ProductGallery
            new Permission
            {
                Id = 81,
                ParentId = 1,
                Title = "مدیریت گالری محصول",
                UniqueName = "ManageProductGallery"
            },
            new Permission
            {
                Id = 82,
                ParentId = 81,
                Title = "لیست گالری محصول",
                UniqueName = "ProductGalleryList"
            },
            new Permission
            {
                Id = 83,
                ParentId = 81,
                Title = "افزودن تصویر جدید",
                UniqueName = "CreateNewProductGallery"
            },
            new Permission
            {
                Id = 84,
                ParentId = 81,
                Title = "حذف تصویر",
                UniqueName = "DeleteProductGallery"
            },
            #endregion

            #region ProductOption
            new Permission
            {
                Id = 85,
                ParentId = 1,
                Title = "مدیریت گزینه های محصول",
                UniqueName = "ManageProductOptions"
            },
            new Permission
            {
                Id = 86,
                ParentId = 85,
                Title = "افزودن گزینه های محصول",
                UniqueName = "CraeteNewProductOption"
            },
            new Permission
            {
                Id = 87,
                ParentId = 85,
                Title = "ویرایش گزینه های محصول",
                UniqueName = "UpdateProductOption"
            },
            new Permission
            {
                Id = 88,
                ParentId = 85,
                Title = "حذف گزینه های محصول",
                UniqueName = "DeleteProductOption"
            },
            #endregion

            #region ProductReview
            new Permission
            {
                Id = 89,
                ParentId = 1,
                Title = "مدیریت بازخورد محصولات",
                UniqueName = "ManageProductReviews"
            },
            new Permission
            {
                Id = 90,
                ParentId = 89,
                Title = "لیست بازخورد های محصولات",
                UniqueName = "ProductReviewsList"
            },
            new Permission
            {
                Id = 91,
                ParentId = 89,
                Title = "تایید بازخورد محصول",
                UniqueName = "ConfirmProductReview"
            },
            new Permission
            {
                Id = 92,
                ParentId = 89,
                Title = "رد بازخورد محصول",
                UniqueName = "RejectProductReview"
            },
            new Permission
            {
                Id = 93,
                ParentId = 89,
                Title = "حذف بازخورد محصول",
                UniqueName = "DeleteProductReview"
            },
            #endregion

            #region Coupon
            new Permission
            {
                Id = 94,
                ParentId = 1,
                Title = "مدیریت کدهای تخفیف",
                UniqueName = "ManageCoupons"
            },
            new Permission
            {
                Id = 95,
                ParentId = 94,
                Title = "لیست کدهای تخفیف",
                UniqueName = "CouponsList"
            },
            new Permission
            {
                Id = 96,
                ParentId = 94,
                Title = "افزودن کد تخفیف جدید",
                UniqueName = "CreateNewCoupon"
            },
            new Permission
            {
                Id = 97,
                ParentId = 94,
                Title = "ویرایش کد تخفیف",
                UniqueName = "UpdateCoupon"
            },
            new Permission
            {
                Id = 98,
                ParentId = 94,
                Title = "حذف کد تخفیف",
                UniqueName = "DeleteCoupon"
            },
            #endregion

            #region PaymentData
            new Permission
            {
                Id = 99,
                ParentId = 1,
                Title = "مدیریت اطلاعات پرداخت",
                UniqueName = "ManagePaymentData"
            },
            new Permission
            {
                Id = 100,
                ParentId = 99,
                Title = "ویرایش اطلاعات پرداخت",
                UniqueName = "UpdatePaymentData"
            },
            #endregion

            #region BalanceTransaction
            new Permission
            {
                Id = 101,
                ParentId = 1,
                Title = "مدیریت تراکنش ها",
                UniqueName = "ManageBalanceTransaction"
            },
            new Permission
            {
                Id = 102,
                ParentId = 101,
                Title = "لیست تراکنش ها",
                UniqueName = "BalanceTransactionsList"
            },
            new Permission
            {
                Id = 103,
                ParentId = 101,
                Title = "افزودن تراکنش جدید",
                UniqueName = "CreateNewBalanceTransaction"
            },
            new Permission
            {
                Id = 104,
                ParentId = 101,
                Title = "حذف تراکنش",
                UniqueName = "DeleteBalanceTransaction"
            },
            #endregion

            #region Order
            new Permission
            {
                Id = 105,
                ParentId = 1,
                Title = "مدیریت سفارش ها",
                UniqueName = "ManageOrders"
            },
            new Permission
            {
                Id = 106,
                ParentId = 105,
                Title = "لیست سفارش ها",
                UniqueName = "OrdersList"
            },
            new Permission
            {
                Id = 107,
                ParentId = 105,
                Title = "ویرایش سفارش",
                UniqueName = "UpdateOrder"
            },
            new Permission
            {
                Id = 108,
                ParentId = 105,
                Title = "جزئیات سفارش",
                UniqueName = "OrderDetails"
            },
            #endregion

            #region Ticket
            new Permission
            {
                Id = 109,
                ParentId = 1,
                Title = "مدیریت تیکت ها",
                UniqueName = "ManageTickets"
            },
            new Permission
            {
                Id = 110,
                ParentId = 109,
                Title = "لیست تیکت ها",
                UniqueName = "TicketsList"
            },
            new Permission
            {
                Id = 111,
                ParentId = 109,
                Title = "جزئیات تیکت",
                UniqueName = "TicketDetails"
            },
            new Permission
            {
                Id = 112,
                ParentId = 109,
                Title = "حذف تیکت",
                UniqueName = "DeleteTicket"
            },
            new Permission
            {
                Id = 113,
                ParentId = 109,
                Title = "ارسال پیام",
                UniqueName = "CreateNewMessage"
            },
            new Permission
            {
                 Id = 114,
                 ParentId = 109,
                 Title = "بستن تیکت",
                 UniqueName = "CloseTicket"
            },
            #endregion

            #region SiteNotifications
            new Permission
            {
                Id = 115,
                ParentId = 1,
                Title = "مدیریت اعلانات",
                UniqueName = "ManageNotifications"
            },
            new Permission
            {
                Id = 116,
                ParentId = 115,
                Title = "لیست اعلانات",
                UniqueName = "NotificationsList"
            },
            #endregion

            #region SmsData
            new Permission
            {
                Id = 117,
                ParentId = 1,
                Title = "مدیریت اطلاعات پنل اس ام اس",
                UniqueName = "ManageSmsData"
            },
            new Permission
            {
                Id = 118,
                ParentId = 117,
                Title = "ویرایش اطلاعات پنل اس ام اس",
                UniqueName = "UpdateSmsData"
            },
            #endregion
        };

        //Last used id ==> 118
        public static List<Permission> Permissions = _permissions;
    }
}
