using System.ComponentModel.DataAnnotations;

namespace CustomerServiceCampaignAPI.Models.Dto
{
    public class PurchaseDTO
    {
        public int Id { get; set; }
        [Required]
        public int AgentId { get; set; }
        [Required]
        public int CustomerId { get; set; }
        [Required]
        public int Price { get; set; }
        public int PriceWithDiscount { get; set; }
    }
}
