namespace Domain.Statics
{
    public class SiteTools
    {
        #region SiteSetting

        public static string SiteName { get; set; }
        public static int MaxImageFileLength { get; set; }
        public static string DefaultImages { get; set; }
        public static string SiteUrl { get; set; }
        public static string SiteMediaUrl { get; set; }

        #endregion

        #region Cache

        public const int CachesDefaultExpirationTimeInMinutes = 180;

        #endregion

        #region Mediator
        public static string MediatorLicenceKey { get; set; }
        #endregion

        #region SiteInfo

        public static string Banner { get; set; }
        public static string OurFeature { get; set; }
        public static string SiteLogo { get; set; }
        public static string BreadcrumbBanner { get; set; }
        public static string Media { get; set; }

        #endregion

        #region Blog

        public static string BlogBanner { get; set; }

        #endregion

        #region Product
        public static string ProductBanner { get; set; }
        public static string ProductIndicator { get; set; }
        #endregion

        #region Payment

        public static string PaymentRequestURL { get; set; }
        public static string CallBackURL { get; set; }
        public static string StartPaymentURL { get; set; }
        public static string VerifyPaymentURL { get; set; }

        #endregion

        #region Telegram

        public static long TelegramErrorChannelId { get; set; }
        public static long TelegramNotificationChannelId { get; set; }
        public static bool TelegramErrorAlertingEnabled { get; set; }
        public static string AdminTelegramWorkerKey { get; set; }
        public static string AdminTelegramWorkerSendUrl { get; set; }

        #endregion
    }
}
