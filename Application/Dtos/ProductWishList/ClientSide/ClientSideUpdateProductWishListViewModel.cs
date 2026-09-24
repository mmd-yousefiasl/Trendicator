using System.ComponentModel.DataAnnotations;

namespace Application.Dtos.ProductWishList.ClientSide
{
    public class ClientSideUpdateProductWishListViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Action { get; set; }
    }
}
