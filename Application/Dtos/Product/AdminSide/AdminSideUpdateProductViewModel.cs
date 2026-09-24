using Application.Dtos;
using Domain.Enums.Products;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Product.AdminSide
{
    public class AdminSideUpdateProductViewModel : CommonDto
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string BannerFileName { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile? BannerFile { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "عنوان در مرورگر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Slug { get; set; }

        [Display(Name = "توضیحات کوتاه")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(500, ErrorMessage = Messages.MaxLengthError)]
        public string ShortDescription { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Description { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "قیمت نمایشی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public long DisplayPrice { get; set; }

        [Display(Name = "قیمت سفارش")]
        public long? DisplayOrderPrice { get; set; }

        [Display(Name = "وضعیت موجودی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsAvailable { get; set; }

        [Display(Name = "دسته بندی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public int CategoryId { get; set; }

        [Display(Name = "نوع")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public ProductTypeEnum Type { get; set; }

        [Display(Name = "لینک متاتریدر")]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string? MetaTraderLink { get; set; }

        [Display(Name = "فایل اندیکاتور")]
        public IFormFile? IndicatorFile { get; set; }

        public string? IndicatorFileName { get; set; }
    }
}
