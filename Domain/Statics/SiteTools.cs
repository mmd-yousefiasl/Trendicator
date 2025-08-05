namespace Domain.Statics
{
    public class SiteTools
    {
        #region SiteSetting

        public static string SiteName { get; set; }
        public static int MaxImageFileLength { get; set; }
        public static string DefaultImages { get; set; }
        public static string SiteUrl { get; set; }

        #endregion

        #region User

        public static string UserAvatarFile { get; set; }

        #endregion

        #region Cache

        public const int CachesDefaultExpirationTimeInMinutes = 60;

        #endregion

        #region SiteInfo

        public static string SliderBanner { get; set; }
        public static string Banner { get; set; }
        public static string OurFeature { get; set; }

        #endregion

        #region Blog

        public static string BlogBanner { get; set; }

        #endregion
    }
}
