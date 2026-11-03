namespace Application.Static
{
    public static class CacheKeys
    {
        public const string BannerCacheKey = "Banner";
        public const string EmailDataCacheKey = "EmailData";
        public const string OurFeatureCacheKey = "OurFeature";
        public const string PaymentDataCacheKey = "PaymentData";
        public const string SiteDataCacheKey = "SiteData";
        public const string HeroCacheKey = "Hero";
        public const string SocialMediaCacheKey = "SocialMedia";
        public const string TestimonialCacheKey = "Testimonial";
        public const string FaqCacheKey = "Faq";
        public const string ProductCategoriesCacheKey = "ProductCategory";
        public const string SiteMapCacheKey = "SiteMap";

        #region Cart
        public const string CartCacheKeyTemplate = "Cart:{0}";
        public static string CartCacheKey(string id)
            => string.Format(CartCacheKeyTemplate, id);
        #endregion
    }
}
