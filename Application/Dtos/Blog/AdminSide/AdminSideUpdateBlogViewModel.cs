using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideUpdateBlogViewModel : CommonDto
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string FileName { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile? File { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "عنوان در مرورگر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Slug { get; set; }

        [Display(Name = "توضیحات متا")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string MetaDescription { get; set; }

        [Display(Name = "بدنه")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public string Body { get; set; }

        [Display(Name = "نویسنده")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Writer { get; set; }

        [Display(Name = "اولویت")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 100, ErrorMessage = Messages.RangeError)]
        public int Priority { get; set; }

        [Display(Name = "وضعیت انتشار")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public bool IsPublished { get; set; }

        [Display(Name = "دسته بندی")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Display(Name = "زمان مطالعه")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [Range(1, 1000)]
        public int TimeToRead { get; set; }
    }
}
