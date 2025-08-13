using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Products
{
    public enum ProductSortTypeEnum
    {
        [Display(Name = "جدید ترین")]
        NewArrivals = 1,
        [Display(Name = "محبوب ترین")]
        Popularity = 2,
        [Display(Name = "قیمت : کمترین به بیشترین")]
        PriceLowToHigh = 3,
        [Display(Name = "قیمت : بیشترین به کمترین")]
        PriceHighToLow = 4
    }
}
