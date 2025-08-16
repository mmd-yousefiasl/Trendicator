using Domain.Enums.Products;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductReview.ClientSide
{
    public class ClientSideWriteProductReviewViewModel
    {
        [Display(Name = "محصول")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int ProductId { get; set; }

        [Display(Name = "بازخورد", Prompt = "نظر شما ...")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(1000, ErrorMessage = Messages.MaxLengthError)]
        public string Review { get; set; }

        [Display(Name = "امتیاز")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public ProductReviewStarsEnum Stars { get; set; } = ProductReviewStarsEnum.FiveStars;

        [Display(Name = "نام شما", Prompt = "نام شما ...")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string UserName { get; set; }

        [Display(Name = "شماره موبایل", Prompt = "شماره موبایل ...")]
        [MaxLength(15, ErrorMessage = Messages.MaxLengthError)]
        [Required(ErrorMessage = Messages.RequiredError)]
        [RegularExpression("^09\\d{9}$", ErrorMessage = Messages.MobileFormatError)]
        public string PhoneNumber { get; set; }

        [Display(Name = "عنوان در مرورگر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Slug { get; set; }
    }
}
