using System.ComponentModel.DataAnnotations;

namespace Application.ViewModels.ProductCompare.ClientSide
{
    public class ClientSideUpdateProductCompareViewModel
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int ProductId { get; set; }

        [Required]
        [MaxLength(10)]
        public string Action { get; set; }
    }
}
