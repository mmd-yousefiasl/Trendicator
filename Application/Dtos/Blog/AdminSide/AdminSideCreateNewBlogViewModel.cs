using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Blog.AdminSide
{
    public class AdminSideCreateNewBlogViewModel
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public IFormFile File { get; set; }

        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "عنوان در مرورگر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(400, ErrorMessage = Messages.MaxLengthError)]
        public string Slug { get; set; }

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

        [Display(Name = "برچسب")]
        public List<int>? Tags { get; set; }
    }
}
