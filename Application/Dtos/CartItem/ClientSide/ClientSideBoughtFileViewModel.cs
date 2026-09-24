using Application.Dtos;
using Domain.Enums.Products;
using Domain.Enums.Shopping;

namespace Application.Dtos.CartItem.ClientSide
{
    public class ClientSideBoughtFileViewModel : CommonDto
    {
        public string ProductTitle { get; set; }
        public string OptionTitle { get; set; }
        public DateTime PaidDate { get; set; }
        public ProductTypeEnum Type { get; set; }
        public CartStatusEnum Status { get; set; }
        public string? MetaTraderLink { get; set; }
    }
}
