using System.ComponentModel.DataAnnotations;

namespace Domain.Enums.Products
{
    public enum ProductReviewStarsEnum
    {
        [Display(Name = "✯✯✯✯✯")]
        FiveStars = 5,
        [Display(Name = "✯✯✯✯✰")]
        FourStars = 4,
        [Display(Name = "✯✯✯✰✰")]
        ThreeStars = 3,
        [Display(Name = "✯✯✰✰✰")]
        TwoStars = 2,
        [Display(Name = "✯✰✰✰✰")]
        OneStar = 1,
    }
}
