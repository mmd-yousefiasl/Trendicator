using Domain.Enums.Products;
using Domain.Models;
using Domain.Shared;
using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.Product.ClientSide
{
    public class ClientSideFilterProductsViewModel : BasePaging<ClientSideProductViewModel>
    {
        [Display(Name = "جست و جو", Prompt = "جستجو کنید ...")]
        [MaxLength(100, ErrorMessage = Messages.MaxLengthError)]
        public string? Search { get; set; }

        public string? Category { get; set; }

        public ProductSortTypeEnum? Sort { get; set; }
    }
}
