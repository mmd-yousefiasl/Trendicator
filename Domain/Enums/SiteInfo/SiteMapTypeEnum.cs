using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.SiteInfo
{
    public enum SiteMapTypeEnum
    {
        [Display(Name = "دسته بندی محصولات")]
        ProductCategory = 1,
        [Display(Name = "محصولات")]
        Product = 2,
        [Display(Name = "دسته بندی مقالات")]
        BlogCategory = 3,
        [Display(Name = "مقالات")]
        Blog = 4,
        [Display(Name = "سایر صفحات")]
        OtherPages = 5
    }
}
