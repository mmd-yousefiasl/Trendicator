using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductWishList.ClientSide
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
