using Domain.Enums.Products;
using Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.CartItem.ClientSide
{
    public class ClientSideFilterMyProductsDto : BasePaging<ClientSideMyProductDto>
    {
        [Display(Name = "کد سفارش", Prompt = "1002...")]
        public int? CartId { get; set; }

        [Display(Name = "نوع محصول")]
        public ProductTypeEnum? Type { get; set; }
    }
}
