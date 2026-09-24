using Domain.Shared;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.Media
{
    public class AdminSideCreateNewMediaViewModel
    {
        [Display(Name = "تصویر")]
        [Required(ErrorMessage = Messages.RequiredError)]
        public IFormFile File { get; set; }
    }
}
