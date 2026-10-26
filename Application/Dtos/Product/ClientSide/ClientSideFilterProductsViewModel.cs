using Domain.Enums.Products;
using Domain.Models;

namespace Application.Dtos.Product.ClientSide
{
    public class ClientSideFilterProductsViewModel : BasePaging<ClientSideProductViewModel>
    {
        public string? Category { get; set; }

        public ProductSortTypeEnum? Sort { get; set; }
    }
}
