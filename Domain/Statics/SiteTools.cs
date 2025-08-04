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
    }
}
