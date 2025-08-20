using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Products
{
    public enum ProductTypeEnum
    {
        [Display(Name = "تریدینگ ویو")]
        TradingView = 1,
        [Display(Name = "لینک متاتریدر")]
        MetaTraderLink = 2,
        [Display(Name = "فایل")]
        File = 3,
    }
}
