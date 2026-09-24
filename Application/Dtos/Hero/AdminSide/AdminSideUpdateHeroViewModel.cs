using Application.Dtos;
using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Hero.AdminSide
{
    public class AdminSideUpdateHeroViewModel : CommonDto
    {
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string Title { get; set; }

        [Display(Name = "توضیحات")]
        [Required(ErrorMessage = Messages.RequiredError)]
        [MaxLength(200, ErrorMessage = Messages.MaxLengthError)]
        public string Description { get; set; }

        [Display(Name = "تصویر")]
        [MaxLength(50, ErrorMessage = Messages.MaxLengthError)]
        public string? FileName { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile? File { get; set; }
    }
}
